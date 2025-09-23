CREATE TABLE [dbo].[TBMedicamentoPrescrito]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [PrescricaoId] UNIQUEIDENTIFIER NOT NULL, 
    [MedicamentoId] UNIQUEIDENTIFIER NOT NULL, 
    [Dosagem] NVARCHAR(100) NOT NULL, 
    [Periodo] NVARCHAR(100) NOT NULL, 
    [Quantidade] INT NOT NULL, 
    CONSTRAINT [FK_TBMedicamentoPrescrito_TBPrescricao] FOREIGN KEY ([PrescricaoId]) REFERENCES [TBPrescricao]([Id]), 
    CONSTRAINT [FK_TBMedicamentoPrescrito_TBMedicamento] FOREIGN KEY ([MedicamentoId]) REFERENCES [TBMedicamento]([Id])
)