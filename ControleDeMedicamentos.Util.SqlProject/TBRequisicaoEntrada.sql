CREATE TABLE [dbo].[TBRequisicaoEntrada]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [DataOcorrencia] DATETIME2 NOT NULL, 
    [FuncionarioId] UNIQUEIDENTIFIER NOT NULL, 
    [MedicamentoId] UNIQUEIDENTIFIER NOT NULL, 
    [QuantidadeRequisitada] INT NOT NULL, 
    CONSTRAINT [FK_TBRequisicaoEntrada_TBFuncionario] FOREIGN KEY ([FuncionarioId]) REFERENCES [TBFuncionario]([Id]),
    CONSTRAINT [FK_TBRequisicaoEntrada_TBMedicamento] FOREIGN KEY ([MedicamentoId]) REFERENCES [TBMedicamento]([Id])
);