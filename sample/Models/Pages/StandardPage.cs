using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;

namespace Knowit.EPiModules.Replaceables.Sample.Models.Pages
{
    [ContentType(DisplayName = "StandardPage", GUID = "ff2277af-4b99-4d52-bcad-4ce92d884b67", Description = "")]
    public class StandardPage : PageData
    {
        [CultureSpecific]
        [Display(
            Name = "Title",
            Description = "The title assosiated to the main body property",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Title { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Main intro",
            Description = "The intro text assosiated to the main body property",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [UIHint(UIHint.Textarea)]
        public virtual string MainIntro { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual XhtmlString MainBody { get; set; }

        [Display(Name = "Content area", Description = "The main content area", Order = 2, GroupName = SystemTabNames.Content)]
        public virtual ContentArea ContentArea { get; set; }
         
    }
}