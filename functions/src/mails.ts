// mails.ts
import { onRequest } from "firebase-functions/v2/https";

import nodemailer from "nodemailer";

// Configuración del SMTP de Duplica
const transporter = nodemailer.createTransport({
  host: "mail.duplica.com",
  port: 465,
  secure: true,
  auth: {
    user: "contacto@redselecta.com",
    pass: "58IZUY@@CtP}Na6$", // mejor ponerlo como variable de entorno en producción
  },
});

// Función HTTPS exportable
export const enviarCorreo = onRequest(async (req, res) => {
  try {
    const { to, subject, html } = req.body;

    if (!to || !subject || !html) {
      res.status(400).json({ success: false, error: "Faltan parámetros: to, subject, html" });
      return;
    }

    const info = await transporter.sendMail({
      from: '"RedSelecta" <contacto@redselecta.com>',
      to,
      subject,
      html,
    });

    res.json({ success: true, messageId: info.messageId });
  } catch (error: any) {
    console.error("Error al enviar correo:", error);
    res.status(500).json({ success: false, error: error.message });
  }
});
