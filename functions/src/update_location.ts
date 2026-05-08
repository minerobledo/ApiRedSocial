import { geohashForLocation } from "geofire-common";
import { GeoPoint } from '@google-cloud/firestore';
import { ProfileData } from "./shared";
import {updateProfileLocateInDB } from './database';
import {obtenerProvinciaDesdeCoordenadas} from './search_early';
/**
/**
 * Updates the location (geopoint and geohash) of a profile when its geopoint changes.
 */
// Asume que las importaciones y funciones auxiliares (obtenerProvinciaDesdeCoordenadas, geohashForLocation, geoPointsAreEqual, ProfileData) están definidas.

// Nota: Asumo que esta función se ejecuta como un Cloud Function onUpdate
export async function updateProfileLocateModule(event: any): Promise<{[key: string]: any;} | null> {
    console.log("🔥 Función de ubicación activada");
    const change = event.data;
    
    if (!change) return null; // Salir si no hay cambios (por seguridad, aunque el trigger lo controla)
    
    const profileId = event.params.profileId;
    const newProfileData = change.after.data() as ProfileData | undefined;
    // Usamos 'before' para verificar si las coordenadas realmente cambiaron
    const oldProfileData = (change.before?.data() as ProfileData | undefined) || {};

    if (!newProfileData) return null;
    
    const updates: { [key: string]: any } = {};
    const promises: Promise<string | null>[] = []; // Para ejecutar llamadas a la API en paralelo

    let calculateProvince1 = false;
    let calculateProvince2 = false;

    // --- Lógica para USUARIO 1 ---
    if (newProfileData.User1GeoPoint) {
        // Verifica si la ubicación cambió (o si el campo de provincia no existe)
        if (!geoPointsAreEqual(newProfileData.User1GeoPoint, oldProfileData.User1GeoPoint) || !newProfileData.User1CurrentProvince) {
            calculateProvince1 = true;
            promises.push(obtenerProvinciaDesdeCoordenadas(
                newProfileData.User1GeoPoint.latitude, 
                newProfileData.User1GeoPoint.longitude
            ));
            
            // Actualizar GeoHash y GeoPoint si es necesario (manteniendo tu lógica original)
            const geohash = geohashForLocation([newProfileData.User1GeoPoint.latitude, newProfileData.User1GeoPoint.longitude], 8);
            updates["User1GeoPoint"] = newProfileData.User1GeoPoint;
            updates["User1GeoHash"] = geohash;
        }
    }

    // --- Lógica para USUARIO 2 ---
    if (newProfileData.User2GeoPoint) {
        // Verifica si la ubicación cambió (o si el campo de provincia no existe)
        if (!geoPointsAreEqual(newProfileData.User2GeoPoint, oldProfileData.User2GeoPoint) || !newProfileData.User2CurrentProvince) {
            calculateProvince2 = true;
            promises.push(obtenerProvinciaDesdeCoordenadas(
                newProfileData.User2GeoPoint.latitude, 
                newProfileData.User2GeoPoint.longitude
            ));
            
            // Actualizar GeoHash y GeoPoint si es necesario (manteniendo tu lógica original)
            const geohash = geohashForLocation([newProfileData.User2GeoPoint.latitude, newProfileData.User2GeoPoint.longitude], 8);
            updates["User2GeoPoint"] = newProfileData.User2GeoPoint;
            updates["User2GeoHash"] = geohash;
        }
    }

    // --- Ejecutar todas las llamadas a la API de Geocodificación en paralelo ---
    const results = await Promise.all(promises);

    // --- Asignar resultados (Crucial: manejo asíncrono y asignación correcta) ---
    let resultIndex = 0;
    
    // Asigna resultado de User 1
    if (calculateProvince1) {
        const province1 = results[resultIndex++]?.toLowerCase() || null;
        if (province1) {
            updates["User1CurrentProvince"] = province1;
        }
    }

    // Asigna resultado de User 2
    if (calculateProvince2) {
        // 🚨 CORRECCIÓN 1: Asegura la asignación al campo User2CurrentProvince
        const province2 = results[resultIndex++]?.toLowerCase() || null;
        if (province2) {
            updates["User2CurrentProvince"] = province2; 
        }
    }

    // Si estás usando la estrategia del campo array 'Provinces' para la búsqueda OR (¡Recomendado!)
    if (updates["User1CurrentProvince"] || updates["User2CurrentProvince"]) {
        const p1 = updates["User1CurrentProvince"] || newProfileData.User1CurrentProvince?.toLowerCase();
        const p2 = updates["User2CurrentProvince"] || newProfileData.User2CurrentProvince?.toLowerCase();
        
        const provincesArray = [p1, p2].filter((p): p is string => !!p);
        
        // 🚨 CORRECCIÓN 2: Asegúrate de tener un campo para la búsqueda OR si lo usas.
        updates["Provinces"] = Array.from(new Set(provincesArray)); 
    }

    // --- 4. Actualizar la Base de Datos ---
    if (Object.keys(updates).length > 0) {
        console.log(`Actualizando perfil ${profileId} con datos de ubicación y provincia:`, updates);
        await updateProfileLocateInDB(updates, profileId) ;
    } else {
        console.log(`No se detectaron cambios de ubicación para ${profileId}.`);
    }

    return null;
}

/**
 * Checks if two GeoPoint objects are equal.
 * @param p1 The first GeoPoint.
 * @param p2 The second GeoPoint.
 * @returns True if the GeoPoints are equal, false otherwise.
 */
function geoPointsAreEqual(p1?: GeoPoint | null, p2?: GeoPoint | null): boolean {
    if (!p1 || !p2) return false;
    return p1?.latitude === p2.latitude && p1.longitude === p2.longitude;
}