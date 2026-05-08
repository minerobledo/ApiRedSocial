import { GeoPoint } from '@google-cloud/firestore';
import { Timestamp } from 'firebase-admin/firestore';
import * as admin from 'firebase-admin';

export interface UserLocation {
    geopoint: GeoPoint;
    geohash: string;
}

export interface ProfileData {
    NameProfile?: string | null;
    Interest?: string | null;
    User1EyeColor?: string | null;
    User2EyeColor?: string | null;
    User1HairType?: string | null;
    User2HairType?: string | null;
    User1Traits?: string | null;
    User2Traits?: string | null;
    User1ZodiacSign?: string | null;
    User2ZodiacSign?: string | null;
    User1EducationLevel?: string | null;
    User2EducationLevel?: string | null;
    User1Gender?: string | null;
    User2Gender?: string | null;
    User1BirthDate?: string | Timestamp | null;
    User2BirthDate?: string | Timestamp | null;
    User1Orientation?: string | null;
    User2Orientation?: string | null;
    User1Province?: string | null;
    User2Province?: string | null;
    User1GeoPoint?: GeoPoint | null;
    User2GeoPoint?: GeoPoint | null;
    User1Location?: UserLocation;
    User2Location?: UserLocation;
    User1CurrentProvince?: string | null;
    User2CurrentProvince?: string | null;
    User1Age?: number;
    User2Age?: number;
    registerUserDtos?: any[];
}

/**
 * Calculates the age from a birthdate.
 * @param fechaNacimiento The birthdate as a string or Timestamp.
 * @returns The age in years.
 */
export function calcularEdad(fechaNacimiento: any): number | null {
    if (!fechaNacimiento) return null;
    const hoy = new Date();
    const nacimiento = fechaNacimiento instanceof admin.firestore.Timestamp
        ? fechaNacimiento.toDate()
        : new Date(fechaNacimiento);

    let edad = hoy.getFullYear() - nacimiento.getFullYear();
    const mes = hoy.getMonth() - nacimiento.getMonth();
    if (mes < 0 || (mes === 0 && hoy.getDate() < nacimiento.getDate())) {
        edad--;
    }
    console.log("dentro de calcularEdad Esta viene siendo la edad: ", edad);
    return edad;
}