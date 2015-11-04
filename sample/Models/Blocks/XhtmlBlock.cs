using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace Knowit.EPiModules.Replaceables.Sample.Models.Blocks
{
    [ContentType(DisplayName = "XhtmlBlock", GUID = "a72304a7-d739-4081-a36f-c6145a9424f1", Description = "")]
    public class XhtmlBlock : BlockData
    {
        
            [CultureSpecific]
            [Display(
                Name = "Content",
                Description = "Name field's description",
                GroupName = SystemTabNames.Content,
                Order = 1)]
            public virtual XhtmlString Content { get; set; }
         
    }
}