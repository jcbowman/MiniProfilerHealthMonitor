MiniProfilerHealthMonitor
=========================

This web application is meant to be UI on top of the popular web profiling tool, Mini Profiler. This application gives the user a easy to understand interface to help gather statistics and pinpoint issues within other web applications. Some of current features are box-and-whisker plots, route frequency use and viewing individual query performance.

This application has been organized in a way to make it easy for anyone to get it up and running in minutes. The only thing required to start reviewing your mini profiler statistics is to change your entity connection string (MiniProfilerEntities) in the web.config.

MiniProfilerHealthMonitor currently uses EF6, ASP.NET MVC 5, Google Visualization (charts) and Twitter Bootstrap 3.
