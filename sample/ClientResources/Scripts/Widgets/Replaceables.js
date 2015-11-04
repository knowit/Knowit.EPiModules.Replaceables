define([
    "dojo/_base/declare",
    "dijit/_WidgetBase",
    "dijit/_TemplatedMixin"
], function (
    declare,
    _WidgetBase,
    _TemplatedMixin
) {
    return declare("knowit.widgets.replaceables",
        [_WidgetBase, _TemplatedMixin], {
            templateString: dojo.cache("/episerver/widgets/replaceables")
        });
});