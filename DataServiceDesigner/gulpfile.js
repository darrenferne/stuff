/// <binding BeforeBuild='copyFilesForBuild' ProjectOpened='default' />
var gulp = require('gulp');
var gulputil = require('gulp-util');
var path = require('path');
var livereload = require('gulp-livereload');

var fileTypes = '\\**\\*.{css,html,js,ico,png,eot,svg,ttf,woff,woff2,otf,gif,map,swf}';

var hostOutputFolder = path.join(__dirname, '/DataServiceDesigner/output');

var bwfFiles = path.join(__dirname, 'node_modules/@brady/bwf') + fileTypes;

var sbModules = path.join(__dirname, '/SchemaBrowser.DataService/modules') + fileTypes;
var sbTemplates = path.join(__dirname, '/SchemaBrowser.DataService/templates') + fileTypes;

var dsdModules = path.join(__dirname, '/DataServiceDesigner.DataService/modules') + fileTypes;
var dsdTemplates = path.join(__dirname, '/DataServiceDesigner.DataService/templates') + fileTypes;

gulp.task('copyFiles', function () {

    gulp.src(bwfFiles)
        .pipe(gulp.dest(hostOutputFolder));
});

gulp.task('copyFilesWithReload', function () {

    // Explorer
    gulp.src(bwfFiles)
        .pipe(gulp.dest(hostOutputFolder))
        .pipe(livereload());

    gulp.src(sbModules)
       .pipe(gulp.dest(path.join(hostOutputFolder, '/dataservices/schemabrowser/modules')))
       .pipe(livereload());
    gulp.src(sbTemplates)
        .pipe(gulp.dest(path.join(hostOutputFolder, '/dataservices/schemabrowser/templates')))
        .pipe(livereload());

    gulp.src(dsdModules)
       .pipe(gulp.dest(path.join(hostOutputFolder, '/dataservices/dataservicedesigner/modules')))
       .pipe(livereload());
    gulp.src(dsdTemplates)
        .pipe(gulp.dest(path.join(hostOutputFolder, '/dataservices/dataservicedesigner/templates')))
        .pipe(livereload());
});

gulp.task('copyFilesForBuild', ['copyFiles', 'copyFilesWithReload'], function () {
    gulputil.log('Copied files to: ' + hostOutputFolder);
});

var toWatch =
    [
        bwfFiles,
        sbModules,
        sbTemplates,
        dsdModules,
        dsdTemplates
    ];

gulp.task('default', ['copyFiles', 'copyFilesWithReload'], function () {
    gulputil.log('Output folder for explorer host: ' + hostOutputFolder);

    livereload.listen();

    gulp.watch(toWatch, ['copyFilesWithReload']);
});
