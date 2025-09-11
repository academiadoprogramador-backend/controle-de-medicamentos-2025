CREATE TABLE [dbo].[TBMedicamento]
(
        [Id]           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Nome]         NVARCHAR(100)    NOT NULL,
        [Descricao]    NVARCHAR(4000)   NOT NULL,
        [FornecedorId] UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT FK_TBMedicamento_TBFornecedor
            REFERENCES [dbo].[TBFornecedor]([Id])
)
