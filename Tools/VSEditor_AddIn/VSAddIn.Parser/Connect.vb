Imports System
Imports Microsoft.VisualStudio.CommandBars
Imports Extensibility
Imports EnvDTE
Imports EnvDTE80

Namespace XeoraCube.VSAddIn
    Public Class Connect
        Implements IDTExtensibility2
        Implements IDTCommandTarget

        Private _applicationObject As DTE2 = Nothing
        Private _addInInstance As AddIn = Nothing
        Private _addInControl As XeoraCube.VSAddIn.AddInControl

        Private WithEvents _textDocumentKeyPressEvents As EnvDTE80.TextDocumentKeyPressEvents

        '''<summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        Public Sub New()

        End Sub

        '''<summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        '''<param name='application'>Root object of the host application.</param>
        '''<param name='connectMode'>Describes how the Add-in is being loaded.</param>
        '''<param name='addInInst'>Object representing this Add-in.</param>
        '''<remarks></remarks>
        Public Sub OnConnection(ByVal application As Object, ByVal connectMode As ext_ConnectMode, ByVal addInInst As Object, ByRef custom As Array) Implements IDTExtensibility2.OnConnection
            If Me._applicationObject Is Nothing Then
                Me._applicationObject = CType(application, DTE2)
                Me._addInInstance = CType(addInInst, AddIn)

                Dim appEvents As EnvDTE80.Events2 = CType(Me._applicationObject.Events, Events2)
                Me._textDocumentKeyPressEvents = CType(appEvents.TextDocumentKeyPressEvents(Nothing), EnvDTE80.TextDocumentKeyPressEvents)

                Me._addInControl = New XeoraCube.VSAddIn.AddInControl(Me._applicationObject, Me._addInInstance)

                AddInLoaderHelper.CreateAppDomain()
            End If
        End Sub

        '''<summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        '''<param name='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        '''<param name='custom'>Array of parameters that are host application specific.</param>
        '''<remarks></remarks>
        Public Sub OnDisconnection(ByVal disconnectMode As ext_DisconnectMode, ByRef custom As Array) Implements IDTExtensibility2.OnDisconnection
            AddInLoaderHelper.DestroyAppDomain()
        End Sub

        '''<summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification that the collection of Add-ins has changed.</summary>
        '''<param name='custom'>Array of parameters that are host application specific.</param>
        '''<remarks></remarks>
        Public Sub OnAddInsUpdate(ByRef custom As Array) Implements IDTExtensibility2.OnAddInsUpdate
        End Sub

        '''<summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        '''<param name='custom'>Array of parameters that are host application specific.</param>
        '''<remarks></remarks>
        Public Sub OnStartupComplete(ByRef custom As Array) Implements IDTExtensibility2.OnStartupComplete
        End Sub

        Public Sub event_TDAfterKeyPressed(ByVal KeyPress As String, ByVal Selection As EnvDTE.TextSelection, ByVal InStatementCompletion As Boolean) Handles _textDocumentKeyPressEvents.AfterKeyPress
            Me._addInControl.event_TDAfterKeyPressed(Nothing, KeyPress, Selection, InStatementCompletion)
        End Sub

        'Public Sub event_CEBeforeExecute(ByVal Guid As String, ByVal ID As Integer, ByVal CustomIn As Object, ByVal CustomOut As Object, ByRef CancelDefault As Boolean) Handles _CommandExecuteEvents.BeforeExecute
        '    Select Case ID
        '        Case 107, 108 'Edit.ListMembers
        '            'MsgBox("deneme")
        '    End Select
        'End Sub

        '''<summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        '''<param name='custom'>Array of parameters that are host application specific.</param>
        '''<remarks></remarks>
        Public Sub OnBeginShutdown(ByRef custom As Array) Implements IDTExtensibility2.OnBeginShutdown
        End Sub

        Public Sub Exec(CmdName As String, ExecuteOption As vsCommandExecOption, ByRef VariantIn As Object, ByRef VariantOut As Object, ByRef Handled As Boolean) Implements IDTCommandTarget.Exec
           
        End Sub

        Public Sub QueryStatus(CmdName As String, NeededText As vsCommandStatusTextWanted, ByRef StatusOption As vsCommandStatus, ByRef CommandText As Object) Implements IDTCommandTarget.QueryStatus
            If NeededText = vsCommandStatusTextWanted.vsCommandStatusTextWantedNone Then
                Select Case CmdName.Substring(CmdName.LastIndexOf("."c) + 1)
                    Case "GotoControlReferance"
                        Me._addInControl.GotoControlReferance(Nothing, Nothing, Nothing)
                End Select
            End If
        End Sub
    End Class
End Namespace