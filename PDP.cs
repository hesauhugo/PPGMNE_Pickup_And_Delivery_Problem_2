using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using System.IO;

namespace Aula5
{
    public class PDP
    {
        public int NumeroDeRequisicoes;
        public int NumeroDeVeiculos;
        public int NumeroDeNos;
        public int NumeroDeArcos;
        public Requisicao[] Requisicoes;
        public Veiculo[] Veiculos;
        public Arco[] Arcos;
        public bool[] Transbordos;
        public bool[,] Adjacencias;
        public GRBEnv Ambiente;
        public GRBModel Modelo;
        public GRBVar[,,] X;
        public GRBVar[,,,] Y;
        public void LerRequisicoes(string Caminho)
        {
            string[] Linhas = File.ReadAllLines(Caminho);
            NumeroDeRequisicoes = Linhas.GetLength(0) - 1;
            Requisicoes = new Requisicao[NumeroDeRequisicoes];
            for (int i = 1; i <= NumeroDeRequisicoes; i++)
            {
                Requisicoes[i - 1] = new Requisicao();
                string[] LinhaAtual = Linhas[i].Split(';');
                Requisicoes[int.Parse(LinhaAtual[0])].LocalColeta = int.Parse(LinhaAtual[1]);
                Requisicoes[int.Parse(LinhaAtual[0])].LocalEntrega = int.Parse(LinhaAtual[2]);
                Requisicoes[int.Parse(LinhaAtual[0])].Quantidade = int.Parse(LinhaAtual[3]);
            }
        }

        public void LerVeiculos(string Caminho)
        {
            string[] Linhas = File.ReadAllLines(Caminho);
            NumeroDeVeiculos = Linhas.GetLength(0) - 1;
            Veiculos = new Veiculo[NumeroDeVeiculos];
            for (int i = 1; i <= NumeroDeVeiculos; i++)
            {
                Veiculos[i - 1] = new Veiculo();
                string[] LinhaAtual = Linhas[i].Split(';');
                Veiculos[int.Parse(LinhaAtual[0])].LocalOrigem = int.Parse(LinhaAtual[1]);
                Veiculos[int.Parse(LinhaAtual[0])].LocalDestino = int.Parse(LinhaAtual[2]);
                Veiculos[int.Parse(LinhaAtual[0])].Capacidade = int.Parse(LinhaAtual[3]);
            }
        }

        public void CriarModelo()
        {
            Ambiente = new GRBEnv();
            Modelo = new GRBModel(Ambiente);
            X = new GRBVar[NumeroDeNos, NumeroDeNos, NumeroDeVeiculos];
            Y = new GRBVar[NumeroDeNos, NumeroDeNos, NumeroDeVeiculos, NumeroDeRequisicoes];
            DefinirVariaveisDeDecisaoFuncaoObjetivo();
            CriarConjuntosRestricoes01();
            CriarConjuntosRestricoes02();
            CriarConjuntosRestricoes03();
            CriarConjuntosRestricoes04();
            CriarConjuntosRestricoes05();
            CriarConjuntosRestricoes06();
            CriarConjuntosRestricoes07();
            CriarConjuntosRestricoes08();
            CriarConjuntosRestricoes09();
        }
        public void DefinirVariaveisDeDecisaoFuncaoObjetivo()
        {
            for (int k = 0; k < NumeroDeVeiculos; k++)
            {
                for (int a = 0; a < NumeroDeArcos; a++)
                {
                    int InicioAtual = Arcos[a].Inicio;
                    int FimAtual = Arcos[a].Fim;
                    double CustoAtual = Arcos[a].Custo[k];
                    X[InicioAtual, FimAtual, k] = Modelo.AddVar(0, 1, CustoAtual, GRB.BINARY, $"x_{InicioAtual}_{FimAtual}_{k}");
                }
            }

            for (int k = 0; k < NumeroDeVeiculos; k++)
            {
                for (int r = 0; r < NumeroDeRequisicoes; r++)
                {
                    for (int a = 0; a < NumeroDeArcos; a++)
                    {
                        int InicioAtual = Arcos[a].Inicio;
                        int FimAtual = Arcos[a].Fim;
                        Y[InicioAtual, FimAtual, k, r] = Modelo.AddVar(0, 1, 0, GRB.BINARY, $"y_{InicioAtual}_{FimAtual}_{k}_{r}");
                    }
                }
            }

        }
        public void CriarConjuntosRestricoes01()
        {
            //Conjunto de restrições que garante que cada veículo sai para no máximo 1 local
            GRBLinExpr expr = new GRBLinExpr();
            for (int k = 0; k < NumeroDeVeiculos; k++)
            {
                expr.Clear();
                int OrigemAtual = Veiculos[k].LocalOrigem;
                for (int j = 0; j < NumeroDeNos; j++)
                {
                    if (Adjacencias[OrigemAtual, j])
                    {
                        expr.AddTerm(1, X[OrigemAtual, j, k]);
                    }
                }
                Modelo.AddConstr(expr <= 1, $"R01_{k}");
            }
        }
        public void CriarConjuntosRestricoes02()
        {
            GRBLinExpr expr1 = new GRBLinExpr();
            GRBLinExpr expr2 = new GRBLinExpr();
            for (int k = 0; k < NumeroDeVeiculos; k++)
            {
                expr1.Clear();
                expr2.Clear();
                int OrigemVeiculo = Veiculos[k].LocalOrigem;
                int DestinoVeiculo = Veiculos[k].LocalDestino;
                for (int j = 0; j < NumeroDeNos; j++)
                {
                    if (Adjacencias[OrigemVeiculo, j])
                    {
                        expr1.AddTerm(1, X[OrigemVeiculo, j, k]);
                    }
                }
                for (int j = 0; j < NumeroDeNos; j++)
                {
                    if (Adjacencias[j, DestinoVeiculo])
                    {
                        expr2.AddTerm(1, X[j, DestinoVeiculo, k]);
                    }
                }
                Modelo.AddConstr(expr1 == expr2, $"R02_{k}");
            }
        }
        public void CriarConjuntosRestricoes03()
        {
            GRBLinExpr expr1 = new GRBLinExpr();
            GRBLinExpr expr2 = new GRBLinExpr();
            for (int k = 0; k < NumeroDeVeiculos; k++)
            {
                for (int i = 0; i < NumeroDeNos; i++)
                {
                    if (i != Veiculos[k].LocalOrigem && i != Veiculos[k].LocalDestino)
                    {
                        expr1.Clear();
                        expr2.Clear();
                        for (int j = 0; j < NumeroDeNos; j++)
                        {
                            if (Adjacencias[i, j])
                            {
                                expr1.AddTerm(1, X[i, j, k]);
                            }
                        }
                        for (int j = 0; j < NumeroDeNos; j++)
                        {
                            if (Adjacencias[j, i])
                            {
                                expr2.AddTerm(1, X[j, i, k]);
                            }
                        }
                        Modelo.AddConstr(expr1 - expr2 == 0, $"R03_{k}_{i}");
                    }
                }
            }
        }
        public void CriarConjuntosRestricoes04()
        {
            GRBLinExpr expr = new GRBLinExpr();
            for (int r = 0; r < NumeroDeRequisicoes; r++)
            {
                expr.Clear();
                int ColetaAtual = Requisicoes[r].LocalColeta;
                for (int k = 0; k < NumeroDeVeiculos; k++)
                {
                    for (int j = 0; j < NumeroDeNos; j++)
                    {
                        if (Adjacencias[ColetaAtual, j])
                        {
                            expr.AddTerm(1, X[ColetaAtual, j, k]);
                        }
                    }
                }
                Modelo.AddConstr(expr == 1, $"R04_{r}");
            }
        }
        public void CriarConjuntosRestricoes05()
        {
            GRBLinExpr expr = new GRBLinExpr();
            for (int r = 0; r < NumeroDeRequisicoes; r++)
            {
                expr.Clear();
                int EntregaAtual = Requisicoes[r].LocalEntrega;
                for (int k = 0; k < NumeroDeVeiculos; k++)
                {
                    for (int j = 0; j < NumeroDeNos; j++)
                    {
                        if (Adjacencias[j, EntregaAtual])
                        {
                            expr.AddTerm(1, X[j, EntregaAtual, k]);
                        }
                    }
                }
                Modelo.AddConstr(expr == 1, $"R05_{r}");
            }
        }
        public void CriarConjuntosRestricoes06()
        {
            GRBLinExpr expr1 = new GRBLinExpr();
            GRBLinExpr expr2 = new GRBLinExpr();
            for (int r = 0; r < NumeroDeRequisicoes; r++)
            {
                for (int i = 0; i < NumeroDeNos; i++)
                {
                    if (Transbordos[i] && i != Requisicoes[r].LocalColeta && i != Requisicoes[r].LocalEntrega)
                    {
                        expr1.Clear();
                        expr2.Clear();
                        for (int k = 0; k < NumeroDeVeiculos; k++)
                        {
                            for (int j = 0; j < NumeroDeNos; j++)
                            {
                                if (Adjacencias[i, j])
                                {
                                    expr1.AddTerm(1, Y[i, j, k, r]);
                                }
                            }
                        }
                        for (int k = 0; k < NumeroDeVeiculos; k++)
                        {
                            for (int j = 0; j < NumeroDeNos; j++)
                            {
                                if (Adjacencias[j, i])
                                {
                                    expr2.AddTerm(1, Y[j, i, k, r]);
                                }
                            }
                        }
                        Modelo.AddConstr(expr1 - expr2 == 0, $"R06_{r}_{i}");
                    }
                }
            }
        }
        public void CriarConjuntosRestricoes07()
        {
            GRBLinExpr expr1 = new GRBLinExpr();
            GRBLinExpr expr2 = new GRBLinExpr();
            for (int k = 0; k < NumeroDeVeiculos; k++)
            {
                for (int r = 0; r < NumeroDeRequisicoes; r++)
                {
                    for (int i = 0; i < NumeroDeNos; i++)
                    {
                        if (Transbordos[i] == false && i != Requisicoes[r].LocalColeta && i != Requisicoes[r].LocalEntrega)
                        {
                            expr1.Clear();
                            expr2.Clear();
                            for (int j = 0; j < NumeroDeNos; j++)
                            {
                                if (Adjacencias[i, j])
                                {
                                    expr1.AddTerm(1, Y[i, j, k, r]);
                                }
                            }
                            for (int j = 0; j < NumeroDeNos; j++)
                            {
                                if (Adjacencias[j, i])
                                {
                                    expr2.AddTerm(1, Y[j, i, k, r]);
                                }
                            }
                            Modelo.AddConstr(expr1 - expr2 == 0, "R07_{k}_{r}_{i}");
                        }
                    }
                }
            }
        }
        public void CriarConjuntosRestricoes08()
        {
            for (int a = 0; a < NumeroDeArcos; a++)
            {
                for (int k = 0; k < NumeroDeVeiculos; k++)
                {
                    for (int r = 0; r < NumeroDeRequisicoes; r++)
                    {
                        int InicioAtual = Arcos[a].Inicio;
                        int FimAtual = Arcos[a].Fim;
                        Modelo.AddConstr(Y[InicioAtual, FimAtual, k, r] <= X[InicioAtual, FimAtual, k], $"R08_{InicioAtual}_{FimAtual}_{k}_{r}");
                    }
                }
            }
        }
        public void CriarConjuntosRestricoes09()
        {
            GRBLinExpr expr = new GRBLinExpr();
            for (int a = 0; a < NumeroDeArcos; a++)
            {
                int InicioAtual = Arcos[a].Inicio;
                int FimAtual = Arcos[a].Fim;
                for (int k = 0; k < NumeroDeVeiculos; k++)
                {
                    expr.Clear();
                    for (int r = 0; r < NumeroDeRequisicoes; r++)
                    {
                        expr.AddTerm(Requisicoes[r].Quantidade, Y[InicioAtual, FimAtual, k, r]);
                    }
                    Modelo.AddConstr(expr <= Veiculos[k].Capacidade * X[InicioAtual, FimAtual, k], $"R09_{InicioAtual}_{FimAtual}_{k}");
                }
            }
        }
    }
    public class Requisicao
    {
        public int LocalColeta;
        public int LocalEntrega;
        public double Quantidade;
    }
    public class Veiculo
    {
        public int LocalOrigem;
        public int LocalDestino;
        public double Capacidade;
    }
    public class Arco
    {
        public int Inicio;
        public int Fim;
        public double[] Custo;
    }
}
