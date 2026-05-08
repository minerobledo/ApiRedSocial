using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;
using static Grpc.Core.Metadata;

namespace Domain.Entities
{
    [FirestoreData]
    public class UsersByProvince
    {
        [FirestoreProperty]
        public double BuenosAires {  get; set; }
        [FirestoreProperty]
        public double Catamarca { get; set; }
        [FirestoreProperty]
        public double Chaco { get; set; }
        [FirestoreProperty]
        public double Chubut { get; set; }
        [FirestoreProperty]
        public double Cordoba { get; set; }
        [FirestoreProperty]
        public double Corrientes { get; set; }
        [FirestoreProperty]
        public double EntreRios {  get; set; }
        [FirestoreProperty]
        public double Formosa { get; set; }
        [FirestoreProperty]
        public double Jujuy { get; set; }
        [FirestoreProperty]
        public double LaPampa {  get; set; }
        [FirestoreProperty]
        public double LaRioja {  get; set; }
        [FirestoreProperty]
        public double Mendoza { get; set; }
        [FirestoreProperty]
        public double Misiones { get; set; }
        [FirestoreProperty]
        public double Neuquen { get; set; }
        [FirestoreProperty]
        public double RioNegro {  get; set; }
        [FirestoreProperty]
        public double Salta { get; set; }
        [FirestoreProperty]
        public double SanJuan { get; set; }
        [FirestoreProperty]
        public double SanLuis {  get; set; }
        [FirestoreProperty]
        public double SantaCruz {  get; set; }
        [FirestoreProperty]
        public double SantaFe {  get; set; }
        [FirestoreProperty]
        public double SantiagoDelEstero { get; set; }
        [FirestoreProperty]
        public double TierraDelFuego { get; set; }
        [FirestoreProperty]
        public double Malvinas { get; set; }
        [FirestoreProperty]
        public double Tucuman { get; set; }
    }
}
