module.exports = {
	entry: {
		ticketList : "./src/ticketList/main.tsx",
		ticket: "./src/ticket/main.tsx"
	},
	devtool: "source-map",
	output: {
		filename: "Scripts/project/support/[name].bundle.js",
		devtoolLineToLine: true,
		sourceMapFilename: "Scripts/project/support/[name].bundle.js.map",
		pathinfo: true
	},
	resolve: {
		extensions: [".Webpack.js", ".web.js", ".ts", ".js", ".tsx"]
	},
	module: {
		loaders: [
				{
					test: /\.tsx?$/,
					exclude: /(node_modules|bower_components)/,
					loader: "ts-loader"
				}
		]
	}
}	