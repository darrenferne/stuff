const gulp = require('gulp');
const path = require('path');
const livereload = require('gulp-livereload');
const normalize = require('normalize-path');

const dirname = normalize(__dirname);

const fileTypes = '\\**\\*.{css,html,js,ico,png,eot,svg,ttf,woff,woff2,otf,gif,map}';
const explorerHostOutputFolder = path.join(dirname, 'output');

const bwfFiles = path.join(dirname, '../node_modules/@brady/bwf') + fileTypes;

console.log('Explorer Host Output Folder:' + explorerHostOutputFolder);
console.log('Bwf Source Folder:' + bwfFiles);

const provisionalContractModules = path.join(dirname, '../Brady.Limits.PhysicalContract.DataService/modules') + fileTypes;
const provisionalContractTemplates = path.join(dirname, '../Brady.Limits.PhysicalContract.DataService/templates') + fileTypes;
console.log('Physical Contract Modules:' + provisionalContractModules);
console.log('Physical Contract Templates:' + provisionalContractTemplates);

const limitsModules = path.join(dirname, '../Brady.Limits.DataService/modules') + fileTypes;
const limitsTemplates = path.join(dirname, '../Brady.Limits.DataService/templates') + fileTypes;
const limitsNancyViews = path.join(__dirname, '../Brady.Limits.DataService/Nancy/Views') + fileTypes;
console.log('Limits Modules:' + limitsModules);
console.log('Limits Templates:' + limitsTemplates);
console.log('Limits Nancy Views:' + limitsNancyViews);

gulp.task('copyBwfFiles', function (done) {
    gulp.src(bwfFiles)
        .pipe(gulp.dest(explorerHostOutputFolder))
        .pipe(livereload({ quiet: true }));

    gulp.src(provisionalContractModules)
        .pipe(gulp.dest(path.join(explorerHostOutputFolder, '/dataservices/provisional_contract/modules')))
        .pipe(livereload({ quiet: true }));

    gulp.src(provisionalContractTemplates)
        .pipe(gulp.dest(path.join(explorerHostOutputFolder, '/dataservices/provisional_contract/templates')))
        .pipe(livereload({ quiet: true }));

    gulp.src(limitsModules)
        .pipe(gulp.dest(path.join(explorerHostOutputFolder, '/dataservices/limitsprototype/modules')))
        .pipe(livereload({ quiet: true }));

    gulp.src(limitsTemplates)
        .pipe(gulp.dest(path.join(explorerHostOutputFolder, '/dataservices/limitsprototype/templates')))
        .pipe(livereload({ quiet: true }));

    gulp.src(limitsNancyViews)
        .pipe(gulp.dest(path.join(explorerHostOutputFolder, '/Nancy/Views')))
        .pipe(livereload({ quiet: true }));

    done();
});

gulp.task('copyAllFiles', gulp.series('copyBwfFiles'));

gulp.task('default', gulp.series('copyAllFiles', function (done) {
    livereload.listen();

    const watching = [
        bwfFiles
    ].map(normalize);

    gulp.watch(watching, gulp.series('copyAllFiles'));
    done();
}));
