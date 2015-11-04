/// <reference path="../Vendor/jquery/jquery-1.10.2.js"/>
/// <reference path="../Vendor/knockout/knockout-3.2.0.js"/>

if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

var replacablesViewModel = {};

//Initialization

$(function() {

    $.get("/episerver/replaceables/", function(data) {
        replacablesViewModel = new ReplaceablesViewModel(data);
        ko.applyBindings(replacablesViewModel);
        $("#waiting-message").hide();
        $("#main-content").show();
    });
});

//Entities

function Replaceable(key, languageCode, value) {
    var self = this;
    //From API
	self.Key = ko.observable(key);
	self.LanguageCode = ko.observable(languageCode);
	self.Value = ko.observable(value);

    //Additional
	self.OriginalValue = ko.observable(value);

    self.Modified = ko.computed(function() {
        return self.Value() !== self.OriginalValue();
    }, self);

    self.Missing = ko.computed(function() {
        return self.Value().substring(0, 1) == "#" && self.Value().substring(self.Value().length - 1, self.Value().length);
    });

    self.Save = function () {

        if (self.Value() == "") {
            alert(ReplaceablesEditor.Locale.Empty);
            return;
        }

        if (self.Value() != self.OriginalValue()) {
            $.ajax({
                url: "/episerver/replaceables/",
                type: "POST",
                data: {
                    Key: self.Key(),
                    LanguageCode: self.LanguageCode(),
                    Value: self.Value(),
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
                },
                success: function() {
                    self.OriginalValue(self.Value());
                    if (replacablesViewModel.Replaceables().indexOf(self) == -1) {
                        (replacablesViewModel.Replaceables().push(self));
                    }
                },
                error: function() {
                    alert(ReplaceablesEditor.Locale.Save);
                }
            });
        }
    };

    self.Reset = function() {
        self.Value(self.OriginalValue());
    };
}

function Language(displayName, languageCode) {
    var self = this;
    self.DisplayName = displayName;
    self.LanguageCode = languageCode;
}


//ViewModel

function ReplaceablesViewModel(data) {
    var self = this;

    self.Replaceables = ko.observableArray($.map(data.Replaceables, function (item) {
        return new Replaceable(item.Key, item.LanguageCode, item.Value);
    }));


    self.Languages = $.map(data.Languages, function(item) { return new Language(item.DisplayName, item.LanguageCode); });


    self.Keys = ko.computed(function () {
        var keys = $.map(self.Replaceables(), function (item) {
            return item.Key();
        });

        var filtered = keys.filter(function (item, i, keys) {
            return i == keys.indexOf(item);
        });

        return filtered;

    }, self);


    self.FindReplaceable = function(key, languageCode) {
        var matches = self.Replaceables().filter(function (item) { return item.Key() == key && item.LanguageCode() == languageCode; });

        if (!matches.length) {
            var missing = new Replaceable(key, languageCode, "#" + key + "#");
            return missing;
        }

        return matches[0];

    };

    self.DeleteKey = function () {
        var key = this.toString();

        if (confirm(ReplaceablesEditor.Locale.Confirm.format(key))) {
            $.ajax("/episerver/replaceables/delete", {
                type: 'POST',
                data: {
                    key: key,
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
                }
            }).done(function() {
                var replaceables = self.Replaceables().filter(function(item) { return item.Key() == key; });

                $.each(replaceables, function(index, item) {
                    self.Replaceables.remove(item);
                });
            }).fail(function(dis) {
                alert(ReplaceablesEditor.Locale.Delete);
            } 
            );


        }
    };

    self.NewKey = ko.observable("");
    self.AddNewKeyError = ko.observable("");
    self.AddNewKey = function () {
        var key = self.NewKey();
        if (!key || key == "") {
            self.AddNewKeyError(ReplaceablesEditor.Locale.KeyMissing);
            return;
        }

        key = key.trim();
        var re = new RegExp('^[a-zA-Z0-9]+$');
        if (!re.test(key)) {
            self.AddNewKeyError(ReplaceablesEditor.Locale.KeyFormatError);
            return;
        }
        var existing = self.Replaceables().filter(function (item) { return item.Key().toLowerCase() == key.toLowerCase(); });
        if (existing.length) {
            self.AddNewKeyError(ReplaceablesEditor.Locale.KeyExists.format(key));
        } else {


            $.each(self.Languages, function (index, item) {
                var replaceable = new Replaceable(key, item.LanguageCode, "");
                self.Replaceables.push(replaceable);
            });
            self.NewKey("");
            self.AddNewKeyError("");
        }
        
    };
}

