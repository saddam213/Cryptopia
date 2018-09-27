<%@ Page Language="C#" %>
<% 
	Response.StatusCode = 404;
	Server.Transfer("~/Errors/404.html");
%>