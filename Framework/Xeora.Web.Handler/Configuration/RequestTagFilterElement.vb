﻿Namespace Xeora.Web.Configuration
    Public Class RequestTagFilterElement
        Inherits System.Configuration.ConfigurationElement

        <System.Configuration.ConfigurationProperty("direction", DefaultValue:=[Shared].Enum.RequestTagFilteringTypes.None, IsRequired:=True)>
        Public ReadOnly Property Direction As [Shared].Enum.RequestTagFilteringTypes
            Get
                Dim rValue As [Shared].Enum.RequestTagFilteringTypes =
                    [Shared].Enum.RequestTagFilteringTypes.None

                [Enum].TryParse(CType(MyBase.Item("direction"), String), rValue)

                Return rValue
            End Get
        End Property

        <System.Configuration.ConfigurationProperty("items", DefaultValue:="&gt;script", IsRequired:=True)>
        Public ReadOnly Property Items As String
            Get
                Return CType(MyBase.Item("items"), String)
            End Get
        End Property

        <System.Configuration.ConfigurationProperty("exceptions")>
        Public ReadOnly Property Exceptions As String
            Get
                Return CType(MyBase.Item("exceptions"), String)
            End Get
        End Property
    End Class
End Namespace