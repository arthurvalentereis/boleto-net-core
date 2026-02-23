using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BoletoNetCore
{
    public class ArquivoRetorno
    {
        public IBanco Banco { get; set; }
        public TipoArquivo TipoArquivo { get; set; }
        public Boletos Boletos { get; set; } = new Boletos();
        public List<SegmentoE> ExtratoBancario { get; set; } = new List<SegmentoE>();
        public HeaderDeLote HeaderDeLote { get; set; }
        public TrailerDeLote TrailerDeLote { get; set; }
        public DateTime? DataGeracao { get; set; }
        public int? NumeroSequencial { get; set; }
        public int? CodigoRemessa { get; set; }
        public int? LayoutArquivo { get; set; }

        private bool _ignorarCarteiraBoleto = false;

        #region Construtores

        public ArquivoRetorno(IBanco banco, TipoArquivo tipoArquivo, bool variasCarteiras = false)
        {
            Banco = banco;
            TipoArquivo = tipoArquivo;
            _ignorarCarteiraBoleto = variasCarteiras;
        }

        /// <summary>
        /// Neste construtor o boleto2net é responsável por atribuir o TipoArquivo e o Banco de acordo com o conteúdo do arquivo de retorno.
        /// O próprio construtor chama o método LerArquivoRetorno2 responsável por carregar/atribuir os boletos e demais informações do arquivo de retorno        /// </summary>
        /// <param name="arquivo">Stream do arquivo de retorno</param>
        public ArquivoRetorno(Stream arquivo)
        {
            LerArquivoRetorno2(arquivo);
        }

        #endregion

        public Boletos LerArquivoRetorno(Stream arquivo)
        {
            Boletos.Clear();
            try
            {
                if (TipoArquivo == TipoArquivo.CNAB400 && Banco.IdsRetornoCnab400RegistroDetalhe.Count == 0)
                    throw new Exception("Banco " + Banco.Codigo.ToString() + " não implementou os Ids do Registro Retorno do CNAB400.");

                using (StreamReader arquivoRetorno = new StreamReader(arquivo, System.Text.Encoding.UTF8))
                {
                    while (!arquivoRetorno.EndOfStream)
                    {
                        var registro = arquivoRetorno.ReadLine();
                        if (TipoArquivo == TipoArquivo.CNAB240)
                        {
                            LerLinhaDoArquivoRetornoCNAB240(registro);
                        }
                        if (TipoArquivo == TipoArquivo.CNAB400)
                        {
                            LerLinhaDoArquivoRetornoCNAB400(registro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler arquivo.", ex);
            }
            return Boletos;
        }
        public void GerarArquivoRemessaMT940(string filePath)
        {
            StringBuilder mt940Content = new StringBuilder();

            // Cabeçalho
            mt940Content.AppendLine("BSBSBRSP");
            mt940Content.AppendLine($":20:{DataGeracao?.ToString("yyyyMMdd")}{CodigoRemessa}");
            mt940Content.AppendLine($":25:{HeaderDeLote.AgenciaConta.ToString().PadLeft(5,'0')}" +
                                        $"{HeaderDeLote.NumeroConta.ToString().PadLeft(12,'0')}" +
                                        $"{HeaderDeLote.DigitoConta}");
            mt940Content.AppendLine($":28C:{HeaderDeLote.SequenciaExtrato}");
            mt940Content.AppendLine($":60F:{HeaderDeLote.SituacaoSaldoInicial}{HeaderDeLote.DataSaldoInicial?.ToString("yyMMdd")}BRL{HeaderDeLote.ValorSaldoInicial.ToString("0.00")}");

            // Transações (exemplo fictício)
            foreach (var transacao in ExtratoBancario)
            {

            mt940Content.AppendLine($":61:{transacao.DataLancamento.ToString("yyMMdd")}{transacao.DataContabil?.ToString("MMdd")}{(char)transacao.TipoLancamento}{HeaderDeLote.TipoMoeda.Substring(2,1)}{transacao.ValorLancamento.ToString("0.00")}{transacao.CodigoHistorico}{transacao.NumeroDocumentoComplemento}");
            mt940Content.AppendLine($":86:{transacao.HistoricoLancamento}");
            }

            // Rodapé
            mt940Content.AppendLine($":62F:{TrailerDeLote.SituacaoSaldoFinal}{TrailerDeLote.DataSaldoFinal?.ToString("yyMMdd")}BRL{TrailerDeLote.ValorSaldoFinal.ToString("0.00")}");

            // Salva o conteúdo no arquivo
            File.WriteAllText(filePath, mt940Content.ToString());
        }
        private void LerArquivoRetorno2(Stream arquivo)
        {

            Boletos.Clear();
            try
            {
                using (StreamReader arquivoRetorno = new StreamReader(arquivo, System.Text.Encoding.UTF8))
                {
                    if (arquivoRetorno.EndOfStream)
                        return;

                    //busca o primeiro registro do arquivo
                    var registro = arquivoRetorno.ReadLine();

                    //atribui o tipo de acordo com o conteúdo do arquivo
                    TipoArquivo = registro.Length == 240 ? TipoArquivo.CNAB240 : TipoArquivo.CNAB400;

                    //instacia o banco de acordo com o codigo/id do banco presente no arquivo de retorno
                    Banco = BoletoNetCore.Banco.Instancia(Utils.ToInt32(registro.Substring(TipoArquivo == TipoArquivo.CNAB240 ? 0 : 76, 3)));

                    if (TipoArquivo == TipoArquivo.CNAB400 && Banco.IdsRetornoCnab400RegistroDetalhe.Count == 0)
                        throw new Exception("Banco " + Banco.Codigo.ToString() + " não implementou os Ids do Registro Retorno do CNAB400.");

                    //define a posicao do reader para o início
                    arquivoRetorno.DiscardBufferedData();
                    arquivoRetorno.BaseStream.Seek(0, SeekOrigin.Begin);

                    while (!arquivoRetorno.EndOfStream)
                    {
                        registro = arquivoRetorno.ReadLine();
                        if (TipoArquivo == TipoArquivo.CNAB240)
                        {
                            LerLinhaDoArquivoRetornoCNAB240(registro);
                        }
                        else
                        if (TipoArquivo == TipoArquivo.CNAB400)
                        {
                            LerLinhaDoArquivoRetornoCNAB400(registro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler arquivo.", ex);
            }

        }

        private void LerLinhaDoArquivoRetornoCNAB240(string registro)
        {
            IBancoCNAB240 b = (IBancoCNAB240)Banco;
            if (b == null) throw new Exception("Leitura CNAB240 não implementada para este banco.");

            var tipoRegistro = registro.Substring(7, 1);
            var tipoSegmento = registro.Substring(13, 1);

            if (tipoRegistro == "0")
            {
                //REGISTRO HEADER DO ARQUIVO RETORNO
                b.LerHeaderRetornoCNAB240(this, registro);
                return;
            }

            if (tipoRegistro == "1")
            {
                // Segmento A - Indica um novo boleto
                var headerLote = new HeaderDeLote();
                b.LerHeaderDeLoteRetornoCNAB240(ref headerLote, registro);
                HeaderDeLote = headerLote;
                return;
            }

            if (tipoRegistro == "3" & tipoSegmento == "T")
            {
                // Segmento T - Indica um novo boleto
                var boleto = new Boleto(this.Banco, _ignorarCarteiraBoleto);
                b.LerDetalheRetornoCNAB240SegmentoT(ref boleto, registro);
                Boletos.Add(boleto);
                return;
            }

            if (tipoRegistro == "3" & tipoSegmento == "U")
            {
                // Segmento U - Continuação do segmento T anterior (localiza o último boleto da lista)
                Boleto boleto = Boletos.LastOrDefault();
                // Se não encontrou um boleto válido, ocorreu algum problema, pois deveria ter criado um novo objeto no registro que foi analisado anteriormente.
                if (boleto == null)
                    throw new Exception("Objeto boleto não identificado");
                b.LerDetalheRetornoCNAB240SegmentoU(ref boleto, registro);
                return;
            }

            if (tipoRegistro == "3" & tipoSegmento == "A")
            {
                // Segmento A - Indica um novo boleto
                var boleto = new Boleto(this.Banco, _ignorarCarteiraBoleto);
                b.LerDetalheRetornoCNAB240SegmentoA(ref boleto, registro);
                Boletos.Add(boleto);
                return;
            }
       

            if (tipoRegistro == "3" & tipoSegmento == "E")
            {
                // Segmento A - Indica um novo boleto
                var segmentoE = new SegmentoE();
                b.LerDetalheRetornoCNAB240SegmentoE(ref segmentoE, registro);
                ExtratoBancario.Add(segmentoE);
                return;
            }

            if (tipoRegistro == "5")
            {
                // Segmento A - Indica um novo boleto
                var trailerLote = new TrailerDeLote();
                b.LerTrailerDeLoteRetornoCNAB240(ref trailerLote, registro);
                TrailerDeLote = trailerLote;
                return;
            }

        }

        private void LerLinhaDoArquivoRetornoCNAB400(string registro)
        {
            IBancoCNAB400 b = (IBancoCNAB400)Banco;
            if (b == null) throw new Exception("Leitura CNAB400 não implementada para este banco.");

            // Identifica o tipo do registro (primeira posição da linha)
            var tipoRegistro = registro.Substring(0, 1);

            // Registro HEADER
            if (tipoRegistro == "0")
            {
                b.LerHeaderRetornoCNAB400(registro);
                return;
            }

            // Registro TRAILER
            if (tipoRegistro == "9")
            {
                b.LerTrailerRetornoCNAB400(registro);
                return;
            }

            // Se o registro não estiver na lista a ser processada pelo banco selecionado, ignora o registro
            if (!Banco.IdsRetornoCnab400RegistroDetalhe.Contains(tipoRegistro))
                return;

            // completa o header com dados do primeiro boleto
            if (this.Boletos.Count == 0)
                b.CompletarHeaderRetornoCNAB400(registro);

            // O primeiro ID da lista, identifica um novo boleto.
            bool novoBoleto = false;
            if (tipoRegistro == Banco.IdsRetornoCnab400RegistroDetalhe.First())
                novoBoleto = true;

            // Se for um novo boleto, cria um novo objeto, caso contrário, seleciona o último boleto
            // Estamos considerando que, quando houver mais de um registro para o mesmo boleto, no arquivo retorno, os registros serão apresentados na sequencia.
            Boleto boleto = null;
            if (novoBoleto)
            {
                boleto = new Boleto(this.Banco, _ignorarCarteiraBoleto);
            }
            else
            {
                if (Boletos.Count > 0)
                    boleto = Boletos[Boletos.Count - 1];

                // Se não encontrou um boleto válido, ocorreu algum problema, pois deveria ter criado um novo objeto no registro que foi analisado anteriormente.
                if (boleto == null)
                    throw new Exception("Objeto boleto não identificado");
            }


            // Identifica o tipo de registro que deve ser analisado pelo Banco.
            switch (tipoRegistro)
            {
                case "1":
                    b.LerDetalheRetornoCNAB400Segmento1(ref boleto, registro);
                    break;
                case "2":
                    b.LerDetalheRetornoCNAB400Segmento2(ref boleto, registro);
                    break;
                case "4":
                    b.LerDetalheRetornoCNAB400Segmento4(ref boleto, registro);
                    break;
                case "7":
                    b.LerDetalheRetornoCNAB400Segmento7(ref boleto, registro);
                    break;
                default:
                    break;
            }

            // Se for um novo boleto, adiciona na lista de boletos.
            if (novoBoleto)
            {
                Boletos.Add(boleto);
            }

        }

        

    }

}

