CREATE TABLE [dbo].[TBPrescricao]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Descricao] NVARCHAR(200) NOT NULL, 
    [DataEmissao] DATETIME2 NOT NULL, 
    [DataValidade] DATETIME2 NOT NULL, 
    [CrmMedico] NVARCHAR(20) NOT NULL, 
    [PacienteId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_TBPrescricao_TBPaciente] FOREIGN KEY (PacienteId) REFERENCES [TBPaciente]([Id]),
)