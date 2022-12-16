using System;
using System.Collections.Generic;
using System.Linq;
using Curso.Data;
using Curso.Domain;
using Curso.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new ApplicationContext();

            // var existe = db.Database.GetPendingMigrations().Any();

            // if(existe){
            //     // NOTIFICAÇÃO OU FINALIZAR TAREFA CASO EXISTA MIGRATIONS PENDENTES
            // }


            // CadastrarPedido();

            // AtualizarDados();

            RemoverRegistro();
        }



        private static void RemoverRegistro()
        {
            using var db = new ApplicationContext();

            // var cliente = db.Clientes.Find(3);
            var cliente = new Cliente
            {
                Id = 3,
            };

            //  db.Clientes.Remove(cliente);
            // db.Remove(cliente);
            
            db.Entry<Cliente>(cliente).State = EntityState.Deleted;

            db.SaveChanges();
        }

        private static void AtualizarDados()
        {
            using var db = new ApplicationContext();
            //  var cliente = db.Clientes.FirstOrDefault(c => c.Id == 3);
            // var cliente = db.Clientes.Find(3);
            var cliente = new Cliente
            {
                Id = 3,
            };

            var clienteDesconectado = new
            {
                Nome = "Mongo Silva",
                Telefone = "12321233",
            };

            db.Attach(cliente);
            db.Entry<Cliente>(cliente).CurrentValues.SetValues(clienteDesconectado);
            // cliente.Nome = "Pedro Rocha Matias";

            // db.Entry<Cliente>(cliente).State = EntityState.Modified;
            // db.Clientes.Update(cliente);

            db.SaveChanges();

        }

        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new ApplicationContext();

            // var  pedidos = db.Pedidos.ToList();  não carrega os items do pedido
            var pedidos = db.Pedidos
                .Include(p => p.Items)
                    .ThenInclude(i => i.Pedido)
                .ToList();

            Console.WriteLine(pedidos);
        }
        private static void CadastrarPedido()
        {
            using var db = new ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();



            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                StatusPedido = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Items = new List<PedidoItem>() {
                    new PedidoItem {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10,
                    }
                }
            };

            db.Pedidos.Add(pedido);

            db.SaveChanges();
        }

        public static void InserirDados()
        {
            var produto = new Produto()
            {
                Descricao = "Produto Teste 3",
                Ativo = true,
                CodigoBarras = "12345678",
                TipoProduto = Curso.ValueObjects.TipoProduto.MercadoriaParaRevenda,
            };

            using var db = new ApplicationContext();

            db.Produtos.Add(produto);
            db.Set<Produto>().Add(produto);
            db.Entry(produto).State = EntityState.Added;
            db.Add(produto);

            var registros = db.SaveChanges();

            Console.WriteLine($"Total registro(s): {registros}");
        }

        private static void InserirDadosMassa()
        {
            var produto = new Produto()
            {
                Descricao = "Produto Teste 3",
                Ativo = true,
                CodigoBarras = "12345678",
                Valor = 10m,
                TipoProduto = Curso.ValueObjects.TipoProduto.MercadoriaParaRevenda,
            };

            var cliente = new Cliente()
            {
                Nome = "Guilherme Pereira",
                CEP = "09435410",
                Cidade = "São Paulo",
                Estado = "SP",
                Telefone = "9984848488"
            };


            using var db = new ApplicationContext();


            db.AddRange(produto, cliente);


            var registros = db.SaveChanges();

            Console.WriteLine($"Total registro(s): {registros}");
        }

        private static void ConsultarDados()
        {
            using var db = new ApplicationContext();

            // var consultarPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            var consultaPorMetodo = db.Clientes
                .AsNoTracking()
                .Where(p => p.Id > 0)
                .OrderBy(p => p.Id)
                .ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id}");
                // db.Clientes.Find(cliente.Id); // consulta primeiramente em memoria
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            }
        }
    }
}
