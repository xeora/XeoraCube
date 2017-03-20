Option Strict On

Namespace Xeora.Web.Handler
    Public Class RequestHandlerFactory
        Implements System.Web.IHttpHandlerFactory

        Public Function GetHandler(ByVal context As System.Web.HttpContext, ByVal requestType As String, ByVal url As String, ByVal pathTranslated As String) As System.Web.IHttpHandler Implements System.Web.IHttpHandlerFactory.GetHandler
            Return New RequestHandler()
        End Function

        Public Sub ReleaseHandler(ByVal handler As System.Web.IHttpHandler) Implements System.Web.IHttpHandlerFactory.ReleaseHandler
            ' Do Nothing
        End Sub
    End Class
End Namespace