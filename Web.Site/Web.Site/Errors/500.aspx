<%@ Page Language="C#" %>
<% 
	Response.StatusCode = 500;
	Server.Transfer("~/Errors/500.html");
%>