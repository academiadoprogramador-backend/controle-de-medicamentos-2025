CREATE TABLE [dbo].[TBMedicamento]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Nome]         NVARCHAR(100)    NOT NULL,
    [Descricao]    NVARCHAR(400)   NOT NULL, 
    [FornecedorId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_TBMedicamento_TBFornecedor] FOREIGN KEY ([FornecedorId]) REFERENCES [TBFornecedor]([Id]),
)
