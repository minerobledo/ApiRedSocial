import * as admin from 'firebase-admin';
import { calcularEdad, ProfileData } from './shared';
import { GeohashRange } from 'geofire-common';
// Initialize Firebase Admin
admin.initializeApp();
const db = admin.firestore();

/**
 * Updates the count of users by province in the database.
 * @param province The province name.
 * @param increment The amount to increment the count by (can be negative).
 */
export async function updateProvinceCount(province: string | null | undefined, increment: number): Promise<void> {
    if (!province) return;
    console.log("dentro de provincia no es nulo");
    const ref = db.collection("Statistics").doc("UsersByProvince");
    console.log("dentro de provincia Se encontro: statistics y UsersByProvince");

    await db.runTransaction(async (tx) => {
        const doc = await tx.get(ref);
        console.log("dentro de provincia hay doc: ", doc.exists);
        const data = doc.exists ? doc.data() as Record<string, number> : {};
        console.log("dentro de provincia hay data: ", data);
        const newCount = Math.max(0, (data[province] || 0) + increment);
        tx.set(ref, { [province]: newCount }, { merge: true });
        console.log("dentro de provincia hay tx.set");
    });
}

/**
 * Updates the count of users by gender in the database.
 * @param sex The gender.
 * @param increment The amount to increment the count by.
 */
export async function updateGenderCount(sex: string | null | undefined, increment: number): Promise<void> {
    if (!sex) return;
    console.log("dentro de updateGender no es nulo");
    const ref = db.collection("Statistics").doc("UserBySex");
    console.log("dentro de updateGender Se encontro: statistics y UserBySex");

    await db.runTransaction(async (tx) => {
        const doc = await tx.get(ref);
        console.log("dentro de updateGender hay doc: ", doc.exists);
        const data = doc.exists ? doc.data() as Record<string, number> : {};
        console.log("dentro de updateGender hay data: ", data);
        const newCount = Math.max(0, (data[sex] || 0) + increment);
        tx.set(ref, { [sex]: newCount }, { merge: true });
        console.log("dentro de updateGender hay tx.set");
    });
}

/**
 * Updates the age of a user in their profile.
 * @param profileId The ID of the profile to update.
 */
export async function updateAge(profileId: string, data: any): Promise<void> {
    const docRef = db.collection("Profiles").doc(profileId);
    console.log("dentro de updateAge se encontró perfil");

    await db.runTransaction(async (tx) => {
        const doc = await tx.get(docRef);
        console.log("dentro de updateAge hay doc: ", doc.exists);
        const data = doc.exists ? doc.data() as ProfileData : {};
        console.log("dentro de updateAge hay data: ", data);
        const updates: { [key: string]: any } = {};

        if (data.User1BirthDate) {
            console.log("dentro de updateAge hay User1BirthDate");
            const nueva = calcularEdad(data.User1BirthDate);
            console.log("dentro de updateAge hay se calculó la edad correctamente para User 1:", nueva);
            if (nueva !== data.User1Age) {
                updates.User1Age = nueva;
                await updateAgeCount(data.User1Age, nueva);
            }
        }

        if (data.User2BirthDate) {
            console.log("dentro de updateAge hay User2BirthDate");
            const nueva = calcularEdad(data.User2BirthDate);
            console.log("dentro de updateAge hay se calculó la edad correctamente para User 2:", nueva);
            if (nueva !== data.User2Age) {
                updates.User2Age = nueva;
                await updateAgeCount(data.User2Age, nueva);
            }
        }
        if (Object.keys(updates).length > 0) {
            tx.set(docRef, updates, { merge: true });
        }
    });
}

/**
 * Updates the count of users by age in the database.
 * @param oldAge The old age (if the user's age changed).
 * @param newAge The new age.
 */
async function updateAgeCount(oldAge: number | null | undefined, newAge: number | null | undefined): Promise<void> {
    const ref = db.collection("Statistics").doc("UserByAge");
    console.log("dentro de updateAgeCount se encontró statistics y UserByAge");

    await db.runTransaction(async (tx) => {
        const doc = await tx.get(ref);
        console.log("dentro de updateAgeCount hay doc: ", doc.exists);
        const data = doc.exists ? doc.data() as Record<string, number> : {};
        console.log("dentro de updateAgeCount hay data: ", data);
        if (oldAge !== null && oldAge !== undefined) {
            tx.set(ref, { [oldAge]: Math.max(0, (data[oldAge] || 0) - 1) }, { merge: true });
        }
        if (newAge !== null && newAge !== undefined) {
            tx.set(ref, { [newAge]: (data[newAge] || 0) + 1 }, { merge: true });
        }
        console.log("dentro de updateAgeCount hay tx.set");
    });
}



/**
 * Updates the count of users by orientation and gender.
 * @param gender The gender.
 * @param orientation The orientation.
 * @param increment The amount to increment the count by.
 */
export async function updateOrientarionCount(gender: string | null | undefined, orientation: string | null | undefined, increment: number): Promise<void> {
    if (!gender || !orientation) return;
    const ref = db.collection("Statistics").doc("UserOrientation");

    return db.runTransaction(async (transaction) => {
        const doc = await transaction.get(ref);
        const data = doc.exists ? doc.data() as Record<string, number> : {};
        const field = `${gender}-${orientation}`;
        const currentCount = data[field] || 0;
        const newCount = Math.max(0, currentCount + increment);
        transaction.set(ref, { [field]: newCount }, { merge: true });
    });
}


export async function updateProfileLocateInDB (updates:{[key: string]: any;}, profileId:string) {
    if (Object.keys(updates).length > 0) {
        await db.collection("Profiles").doc(profileId).update(updates);
    } 
};

export async function updateProfileGeoHash(b : GeohashRange): Promise<any> {
    const q1 = db.collection('Profiles').orderBy('User1GeoHash').startAt(b[0]).endAt(b[1]);
    const q2 = db.collection('Profiles').orderBy('User2GeoHash').startAt(b[0]).endAt(b[1]);

    return {q1, q2};
}

export async function getProfileByCurrentProvince(centroProv : string | null): Promise<any> {
    const q1 = db.collection('Profiles').where('User1CurrentProvince', '==', centroProv)
            .limit(50);
    const q2 = db.collection('Profiles').where('User2CurrentProvince', '==', centroProv)
            .limit(50);

    return {q1, q2};
}

export async function getContestsSnap(): Promise<any> {
    return await db.collection("Contests").get();
}

export async function getBatchDB(): Promise<any> {
    return await db.batch();
}

export async function getPosts(id : any): Promise<any> {
    return await db.doc(`Posteos/${id}`).get();
}

export async function getCollection(collection : string, fieldName? : any, operator? : any, valueComp? : any): Promise<any> {
    if(fieldName) {
        return await db.collection(collection).where(fieldName, "==", valueComp).get();
    }
    return db.collection(collection);
}