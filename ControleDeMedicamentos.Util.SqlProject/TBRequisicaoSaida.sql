CREATE TABLE [dbo].[TBRequisicaoSaida]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [DataOcorrencia] DATETIME2 NOT NULL, 
    [FuncionarioId] UNIQUEIDENTIFIER NOT NULL, 
    [PrescricaoId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_TBRequisicaoSaida_TBFuncionario] FOREIGN KEY ([FuncionarioId]) REFERENCES [TBFuncionario]([Id]),
    CONSTRAINT [FK_TBRequisicaoSaida_TBPrescricao] FOREIGN KEY ([PrescricaoId]) REFERENCES [TBPrescricao]([Id])
)