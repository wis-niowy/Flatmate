var rimraf = require("rimraf");

//Deletes all the directories with their content from the list below.

const directories = ['package-lock.json', 'node_modules'];

for (const dir of directories) {
    rimraf(dir, function () { console.log("Directory: " + dir + " removed."); });
}