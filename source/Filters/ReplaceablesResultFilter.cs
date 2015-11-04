using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.Logging;
using Knowit.EPiModules.Replaceables.Helper;
using LogManager = EPiServer.Logging.LogManager;

namespace Knowit.EPiModules.Replaceables.Filters
{
    internal class ReplaceablesResultFilter : IResultFilter
    {

        private readonly ILogger Log = LogManager.GetLogger(typeof(ReplaceablesResultFilter));

        private class Writers
        {
            public StringBuilder Builder { get; set; }
            public TextWriter Output { get; set; }
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!FilterApplies(filterContext)) return;

            var requestId = Guid.NewGuid();
            filterContext.RequestContext.HttpContext.Items["request-id"] = requestId;

            //Hijacks the output so that its saved to our local variable
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var output = filterContext.RequestContext.HttpContext.Response.Output;

            var writers = new Writers()
            {
                Builder = stringBuilder,
                Output = output
            };

            filterContext.HttpContext.Items[requestId] = writers;

            filterContext.RequestContext.HttpContext.Response.Output = stringWriter;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (!FilterApplies(filterContext)) return;

            try
            {

                if (filterContext.RequestContext.HttpContext.Items.Contains("request-id"))
                {
                    Guid requestId;

                    if (Guid.TryParse(filterContext.RequestContext.HttpContext.Items["request-id"].ToString(), out requestId)
                        && filterContext.RequestContext.HttpContext.Items.Contains(requestId))
                    {
                        var writers = filterContext.RequestContext.HttpContext.Items[requestId] as Writers;
                        if (writers != null)
                        {
                            var StopWatch = new Stopwatch();
                            string response = string.Empty;
                            StopWatch.Restart();
                            response = ReplaceableHelper.SetReplacables(writers.Builder.ToString());
                            StopWatch.Stop();
                            var ticksPerSecond = Stopwatch.Frequency;
                            var ticksPerMilliSecond = ticksPerSecond/1000;
                            var time = ((double)StopWatch.ElapsedTicks)/ticksPerMilliSecond;

                            Log.Information(string.Format("Time to replace: {0} ms", time.ToString("F4")));

                            //Get the output up to date to where it should have been
                            writers.Output.Write(response);

                            //Returns the hijacked output so that it dosnt cause further problems
                            filterContext.RequestContext.HttpContext.Response.Output = writers.Output;

                            return;
                        }
                        Log.Error("Writers is null");
                        throw new Exception("Writers is null");
                    }
                    Log.Error("Request-id in HttpContext.Items is not a valid guid, or its value is missing");
                    throw new Exception("Request-id in HttpContext.Items is not a valid guid, or its value is missing");
                }

                Log.Error("Missing request-id in HttpContext.Items");
                throw new Exception("Missing request-id in HttpContext.Items");

            }
            catch (Exception e)
            {
                Log.Error("Failed when trying to run replaceables:" + filterContext.HttpContext.Request.Url, e);
                throw;
            }


        }

        private bool FilterApplies(ControllerContext context)
        {
            var executed = context as ResultExecutedContext;
            var executing = context as ResultExecutingContext;

            ActionResult result = null;

            if (executing != null) result = executing.Result;
            else if (executed != null) result = executed.Result;


            if (context.IsChildAction) return false;

            if (context.Controller.GetOriginalType().HasAttribute<ReplaceablesBypass>()) return false;

            if (context.HttpContext.Request.Params["epieditmode"] != null && IsReplaceable(result)) return true; //Makes filter apply when you edit or preview a episerver page or block

            if (context.HttpContext.Request.Path.StartsWith("/episerver", StringComparison.OrdinalIgnoreCase)) return false; //Makes filter not apply when internal episerver things other than pages and blocks

            return IsReplaceable(result);
        }

        private bool IsReplaceable(ActionResult result)
        {
            return result != null
                   && (result is ViewResult
                       || result is PartialViewResult
                       || result is JsonResult
                       || result is ContentResult);
        }
    }
}