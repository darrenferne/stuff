/// <binding ProjectOpened='default' />
const gulp = require('gulp');
const path = require('path');
const livereload = require('gulp-livereload');
const normalize = require('normalize-path');

const dirname = normalize(__dirname);
const hostOutputFolder = path.join(__dirname, '/DataServiceDesigner/output');
const fileTypes = '\\**\\*.{css,html,js,ico,png,eot,svg,ttf,woff,woff2,otf,gif,map,swf}';

const bwfFiles = path.join(__dirname, 'node_modules/@brady/bwf') + fileTypes;

const sbModules = path.join(__dirname, '/SchemaBrowser.DataService/modules') + fileTypes;
const sbTemplates = path.join(__dirname, '/SchemaBrowser.DataService/templates') + fileTypes;

const dsdModules = path.join(__dirname, '/DataServiceDesigner.DataService/modules') + fileTypes;
const dsdTemplates = path.join(__dirname, '/DataServiceDesigner.DataService/templates') + fileTypes;

console.log("Dir Name: " + dirname);
console.log("Host Output Folder: " + hostOutputFolder);
console.log("Bwf Files: " + bwfFiles);
console.log("Schema Browser Modules: " + sbModules);
console.log("Schema Browser Templates: " + sbTemplates);
console.log("Data Service Designer Modules: " + dsdModules);
console.log("Data Service Designer: " + dsdTemplates);

gulp.task('copyAllFiles', function (done) {
	gulp.src(bwfFiles)
        .pipe(gulp.dest(hostOutputFolder))
        .pipe(livereload());

    gulp.src(sbModules)
        .pipe(gulp.dest(path.join(hostOutputFolder, '/dataservices/schemabrowser/modules')))
        .pipe(livereload({ quiet: true }));

    gulp.src(sbTemplates)
        .pipe(gulp.dest(path.join(hostOutputFolder, '/dataservices/schemabrowser/templates')))
        .pipe(livereload({ quiet: true }));

	gulp.src(dsdModules)
        .pipe(gulp.dest(path.join(hostOutputFolder, '/dataservices/dataservicedesigner/modules')))
        .pipe(livereload({ quiet: true }));

    gulp.src(dsdTemplates)
        .pipe(gulp.dest(path.join(hostOutputFolder, '/dataservices/dataservicedesigner/templates')))
        .pipe(livereload({ quiet: true }));
		
    done();
});

gulp.task('default', gulp.series('copyAllFiles'));


