using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Muhendislik_4
{
    public partial class Form2 : Form
    {
        public DataTable veri { get; set; }//form1 den gelen verileri karşılar
        public int arkadasSayisi { get; set; }

        public double[] ebias = new double[16];//bias dizileri
        public double[] ybias = new double[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1 };

        DataTable egitim;//iki adet datasetimiz
        DataTable test;


        public Form2()
        {
            InitializeComponent();
        }
        private void Form2_Load_1(object sender, EventArgs e)
        {

            //Oledb bağlantımız excell den öğrenci numara ve isim çekmek için
            OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source =C:\\Users\\P1274\\Desktop\\University\\Mühendislik Projesi\\Mühendislik Projesi 4\\ogrencilistesi.xlsx; Extended Properties = Excel 12.0");
            baglanti.Open();
            string sorgu = "select * from [Sheet1$] ";
            OleDbDataAdapter data_adaptor = new OleDbDataAdapter(sorgu, baglanti);
            baglanti.Close();

            DataTable ogreni_listesi = new DataTable();
            data_adaptor.Fill(ogreni_listesi);
            //excell den çekilen data ogrenci_listesi data table a atılır

            //form1 den gelen toplam veri datalojistik gridine atılır
            DataLojistik.DataSource = veri;
            int sayi = arkadasSayisi;//arkadaş sayısı alnır

            int sayi1 = DataLojistik.Rows.Count;

            egitim = new DataTable();//data table ların nesneleri oluşturulur
            test = new DataTable();

            for (int k = 0; k < 17; k++)
            {//2 adet datatable ımızın headerları oluşturulur
                egitim.Columns.Add(new DataColumn(k.ToString()));
                test.Columns.Add(new DataColumn(k.ToString()));
            }


            //ilk datagridimizde toplam kaç kişi olacağı belirlenir
            int ilk_dongu = (DataLojistik.Rows.Count - 1 - arkadasSayisi) / 2;

           
            
            //for döngüsünde bu belirlenir
            for (var i = 0; i< ilk_dongu + arkadasSayisi; i++)
            {
                //arkadaş olanlar ve kalan verinin yarısı kadar döner
                //bunları egitim data table a atılır
                DataRow row = egitim.NewRow();
                for (var j = 0; j < DataLojistik.Rows[i].Cells.Count;j++)
                {
                    row[j] = DataLojistik.Rows[i].Cells[j].Value;
                }
                egitim.Rows.Add(row);
            }

            for (var i = ilk_dongu + arkadasSayisi; i < DataLojistik.Rows.Count-1; i++)
            {//kalan datayı test data table a atılır
                DataRow row1 = test.NewRow();
                for (var j = 0; j < DataLojistik.Rows[i].Cells.Count; j++)
                {
                    row1[j] = DataLojistik.Rows[i].Cells[j].Value;
                }
                test.Rows.Add(row1);
            }

            //datalojistik gridi eğitim verisi
            DataLojistik.DataSource = egitim;
            //datatest gridi uygulanacak test verisi
            dataTest.DataSource = test;



            //bias hesaplama//
            double reg = 0;
            for(var i = 0; i < 100; i++)
            {//100 iterasyon sayısı
                reg = 0;
                for(var j = 0; j < DataLojistik.Rows.Count-1; j++)
                {//eğitim verisi üzerinde dön
                    //bu kısımda bias[0] hesaplanır
                    reg = reg + bias(DataLojistik.Rows[j]) - Convert.ToDouble(DataLojistik.Rows[j].Cells[16].Value);
                }
                ebias[0] = ybias[0] - (0.0001* (1 / ((double)DataLojistik.Rows.Count-1)) * reg);
                reg = 0;
                for (var q = 1; q <= 15; q++)
                {//bu kısımda diğer bias değerleri hesaplanır
                    for (var j = 0; j < DataLojistik.Rows.Count-1; j++)
                    {
                        reg = reg + ( bias(DataLojistik.Rows[j]) - Convert.ToDouble(DataLojistik.Rows[j].Cells[16].Value) ) * Convert.ToDouble(DataLojistik.Rows[j].Cells[q].Value);
                        //reg = reg * Convert.ToDouble(DataLojistik.Rows[j].Cells[q].Value);
                    }
                    ebias[q] = ybias[q] - (0.0001 * (1 / ((double)DataLojistik.Rows.Count-1)) * reg);
                    reg = 0;
                }

                for(var z = 0; z<16; z++)
                {//son olarak güncel bias değerleri ybias a atılır
                    ybias[z] = ebias[z];
                }
            }

            for (var j = 0; j < dataTest.Rows.Count-1; j++)
            {//test verisi üzerinde bias değerleri kullanılır
                dataTest.Rows[j].Cells[16].Value = bias(dataTest.Rows[j]).ToString();
            }



            double[] Arkadaslar = new double[dataTest.Rows.Count-1];//En büyük değer çıkan 10 arkadaş
            string[] arkadasnumara = new string[dataTest.Rows.Count-1];//hesaplama

            for (var i = 0;i< dataTest.Rows.Count-1;i++)
            {//for içiersinde dönerek tüm datayı bu dizilere atılır
                Arkadaslar[i] = Convert.ToDouble(dataTest.Rows[i].Cells[16].Value);
                arkadasnumara[i] = dataTest.Rows[i].Cells[0].Value.ToString();
               
            }

            int minindis;
            double temp;
            string temp1;
            for(var i = 0; i< Arkadaslar.Length; i++)
            {//dizi büyük biasdan küçüğe doğru sıralanır
                for(var j = 0; j< Arkadaslar.Length; j++)
                {
                    if ( Arkadaslar[j] < Arkadaslar[i])
                    {
                        temp = Arkadaslar[i];
                        Arkadaslar[i] = Arkadaslar[j];
                        Arkadaslar[j] = temp;

                        temp1 = arkadasnumara[i];
                        arkadasnumara[i] = arkadasnumara[j];
                        arkadasnumara[j] = temp1;
                    }   
                }
            }

            string msg = "Öğrenci Numara\tÖğrenci İsim\n\n";
            string isim = "";
            for (int i = 0; i < 10; i++)
            {   //mesajda ilk 10 öğrenci numarası ve ismi yazdırılır
                for(var j = 0; j < ogreni_listesi.Rows.Count; j++)
                {
                    if (ogreni_listesi.Rows[j][0].ToString().Equals(arkadasnumara[i].ToString()))
                    {
                        isim = ogreni_listesi.Rows[j][1].ToString();
                    }
                }
                //Öğrenci isim ve numaraları bir düzene göre sıralanır
                msg += (i+1)+"-) "+arkadasnumara[i].ToString()+"\t"+isim+"\n";
            }
            MessageBox.Show(msg);
        }
        public double bias(DataGridViewRow g1)
        {//bias fonksiyonu hesaplaması
            double sonuc;
            double e = ybias[0];
            for(var i = 1; i < 16; i++)
            {
                e = e + ( Convert.ToDouble(g1.Cells[i].Value) * ybias[i] );
            }
            e = -1 * e;
            double b1 = Math.Exp(e);
            b1++;
            sonuc = 1 / b1;
            return sonuc;
        }
        
    }
}
