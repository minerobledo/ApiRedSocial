import { onRequest } from "firebase-functions/v2/https";
import { onDocumentWritten, onDocumentDeleted } from "firebase-functions/v2/firestore";
import { updateStatics } from './update_statics';
import { updateProfileLocateModule } from './update_location';
import { buscarPerfilesCercanosModule } from './search_early';
import { postDelete } from "./post_delete";
import { borrarReferenciasDePerfilModule } from "./update_profile";
import {enviarCorreo} from "./mails";
import net from "net";

/**
 * Updates statistics (gender, age, province) when a profile is created, updated, or deleted.
 */
export const updateStatistics = onDocumentWritten("Profiles/{profileId}", async (event) => {
    await updateStatics(event);
    await updateProfileLocateModule(event);
});

//
export const onPostDelete =  onDocumentDeleted("Post/{postId}", async (event) => {
    await postDelete(event);
  });

export const borrarReferenciasDePerfil = onDocumentDeleted("Profiles/{profileId}", async (event) => {
    await borrarReferenciasDePerfilModule(event);
  });

/*
 * Searches for profiles within a given radius of a center point, optionally filtered by age and gender.
 */
export const buscarPerfilesCercanos = onRequest(async (req, res) => {
    await buscarPerfilesCercanosModule(req, res);
});


export const enviarCorreoFunction = enviarCorreo; // ya queda disponible para ser llamada desde HTTP

export const testSMTPConnection = onRequest(async (req, res) => {
  const host = "mail.duplica.com";
  const port = 465;

  const socket = new net.Socket();
  const timeout = 5000; // 5 segundos

  socket.setTimeout(timeout);

  socket.on("connect", () => {
    socket.destroy();
    res.send(`Conexión exitosa a ${host}:${port}`);
  });

  socket.on("timeout", () => {
    socket.destroy();
    res.send(`Timeout: no se pudo conectar a ${host}:${port}`);
  });

  socket.on("error", (err) => {
    socket.destroy();
    res.send(`Error al conectar a ${host}:${port}: ${err.message}`);
  });

  socket.connect(port, host);
});
