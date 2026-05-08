import { getBatchDB, getCollection } from './database';
import * as admin from 'firebase-admin';

export async function borrarReferenciasDePerfilModule(event : any): Promise<void> {
  const perfilId = event.params.profileId;
  
  let batch = await getBatchDB();

  // Colecciones principales donde buscar y eliminar documentos o referencias
  const collectionsToClean = [
      { collection: "Chat", field: "Profile1" },
      { collection: "Chat", field: "Profile2" },
      { collection: "EventEntity", field: "GuestList", type: "array", action: "remove" },
      { collection: "Frienships", field: "Friend1Id" },
      { collection: "Frienships", field: "Friend2Id" },
      { collection: "Notifications", field: "ProfileId" },
      { collection: "Posts", field: "IdPublisher" },
      { collection: "RefreshToken", field: "ProfileId" }
  ];

  // Función para eliminar documentos en una colección basados en un campo
  async function processCollection(collectionName: string, fieldName: string) {

      const querySnapshot = await getCollection(collectionName, fieldName, "==", perfilId);

      for (const doc of querySnapshot.docs) {
          batch.delete(doc.ref);

          // Si la colección es "Chat", también eliminamos los documentos de la subcolección "Messages"
          if (collectionName === "Chat") {
              const messagesSnapshot = await doc.ref.collection("Messages").get();
              messagesSnapshot.forEach((messageDoc : any) => {
                  batch.delete(messageDoc.ref);
              });
          }
      }
  }

  // Función para eliminar documentos en una colección donde un array contiene el perfilId
  async function processCollectionArray(collectionName: string, fieldName: string) {

      const querySnapshot = await getCollection(collectionName, fieldName, "array-contains", perfilId);

      for (const doc of querySnapshot.docs) {
          batch.update(doc.ref, {
              [fieldName]: admin.firestore.FieldValue.arrayRemove(perfilId)
          });
      }
  }

  // Procesar las colecciones principales
  for (const entry of collectionsToClean) {
      if (entry.type === "array" && entry.action === "remove") {
          await processCollectionArray(entry.collection, entry.field);
      } else {
          await processCollection(entry.collection, entry.field);
      }
  }

  await batch.commit();
  console.log(`Se eliminaron las referencias al perfil ${perfilId} en las colecciones especificadas y sus subcolecciones "Messages" en "Chat".`);

}