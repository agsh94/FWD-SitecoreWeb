define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js", "/-/speak/v1/FXM/ExperienceEditorExtension/Legacy/LegacyjQuery.js"], function (Sitecore, ExperienceEditor, $sc) {
    return {
        priority: 1,
        execute: function (context) {
            $sc(document).ready(function (event) {
                console.log("Inside Custom DropLink");
            });
        }
    };
});