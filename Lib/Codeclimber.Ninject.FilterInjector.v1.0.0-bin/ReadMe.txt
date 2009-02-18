-----------------------------------
    Filter Injector for Ninject
-----------------------------------

 - Introduction
This is small utility library that allows you to inject dependencies
inside ASP.NET MVC ActionFilters.

The AutoControllerAndFilterModule is a modification of the AutoControllerModule
that comes with the ASP.NET MVC integration library of Ninject.

- How to use it
Everything is exactly the same as when using ASP.NET MVC togheter with Ninject.
The only difference is that you have to replace the call to AutoControllerModule to
AutoControllerAndFilterModule.

If you don't know how to configure ASP.NET MVC with Ninject, please consider reading my
tutorial on my blog:
http://codeclimber.net.nz/archive/2009/02/05/how-to-use-ninject-with-asp.net-mvc.aspx

I also wrote a post explaining in detail how to use this extension:
http://codeclimber.net.nz/archive/2009/02/15/extending-ninject-to-inject-dependencies-into-action-filters.aspx


- Getting the code
The source code is available online on my SVN repository on Google Code:
http://code.google.com/p/codeclimber/



If you didn't already, consider subscribing to the RSS feed at:
http://feeds2.feedburner.com/Codeclimber

Copyright (c) 2009, Simone Chiaretta