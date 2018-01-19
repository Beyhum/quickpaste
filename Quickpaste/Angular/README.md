# QuickpasteApp

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 1.5.2.

## Custom changes to setup Angular folder in ASP.NET Core

1. Run 'ng eject' after creating new Angular app and move all files to 'Angular' folder in ASP.NET Core project
2. Change the following in .angular-cli.json (outDir and index values):
```javascript
"apps": [
    {
      "outDir": "../wwwroot",
      "index": "../../Views/Shared/_WebpackTemplate.cshtml",
      ...      
    }
  ],
```
3. Change protractor.conf.js 'baseUrl' value to ASP.NET Core's dev url/port (default is localhost:44340):
```javascript
  baseUrl: 'http://localhost:44340/',
```
4. Change webpack.config.js 'output' and 'plugins' to the following:
```javascript
module.exports = {
  ...
  "output": {
          "path": path.join(process.cwd(), "../wwwroot/dist"),
          "filename": "[name].bundle.js",
          "chunkFilename": "[id].chunk.js",
          "crossOriginLoading": false,
          "publicPath":  "/dist/"
  },
  ...
  "plugins": [
    ...,
    new HtmlWebpackPlugin({
        "template": "../Views/Shared/_WebpackTemplate.cshtml",
        "filename": "./../../Views/Home/Index.cshtml",
        ...
      },
    ...
  ]
}
```
5. Run the following commands in your new angular project to install Hot Module Reloading
```
npm install --save-dev aspnet-webpack 
npm install --save-dev webpack-hot-middleware
```
6. Add the following lines to the beginning of your main.ts file:
``` javascript
if (module['hot']) {
    module['hot'].accept();
}
```

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build, Unit tests, e2e tests

To run your builds, you now need to do the following commands:
- "npm run build" to build. (build artifacts will be stored in the .NET Core wwwroot/dist folder)
- "npm run build-prod" to build for production. (build artifacts will be stored in the .NET Core wwwroot/dist folder)
- "npm test" to run unit tests.
- "npm run e2e" to run protractor.
