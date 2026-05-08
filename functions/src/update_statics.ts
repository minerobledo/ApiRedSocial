import { ProfileData } from './shared';
import { updateProvinceCount, updateGenderCount, updateAge, updateOrientarionCount } from './database'
/**
 * Updates statistics (gender, age, province) when a profile is created, updated, or deleted.
 */
export async function updateStatics(event : any): Promise<any> {
    console.log("entra en update statics");
    const profileId = event.params.profileId;
    console.log("obtiene params statics");
    const change = event.data;
    console.log("obtiene data statics");

    if (!change) {
        console.log("Change is null, exiting");
        return null;
    }

    console.log("no es nulo en statics");

    const newProfileData = change.after.data() as ProfileData | undefined;
    console.log("obtiene after: ", newProfileData);

    const oldProfileData = change.before?.data() as ProfileData | undefined | null;
    console.log("obtiene before: ", oldProfileData);

    if (!change.after.exists) {
        console.log(`Perfil ${profileId} eliminado.`);
        if (oldProfileData?.User1Province) {
            await updateProvinceCount(oldProfileData.User1Province, -1);
        }
        if (oldProfileData?.User2Province) {
            await updateProvinceCount(oldProfileData.User2Province, -1);
        }
        return;
    }

    if (!oldProfileData) {

        if (newProfileData?.User1Gender) {
            console.log("Genero 1");
            await updateGenderCount(newProfileData.User1Gender, 1);
        }
        if (newProfileData?.User2Gender) {
            console.log("Genero 2");
            await updateGenderCount(newProfileData.User2Gender, 1);
        }
        if (newProfileData?.User1BirthDate) {
            console.log("Cumpleanos 1");
            await updateAge(profileId, newProfileData);
        }
        if (newProfileData?.User2BirthDate) {
            console.log("Cumpleanos 2");
            await updateAge(profileId,newProfileData);
        }
        if (newProfileData?.User1Orientation && newProfileData?.User1Gender) {
            console.log("Es traba 1");
            await updateOrientarionCount(newProfileData.User1Gender, newProfileData.User1Orientation, 1);
        }
        if (newProfileData?.User2Orientation && newProfileData?.User2Gender) {
            console.log("Es traba 2");
            await updateOrientarionCount(newProfileData.User2Gender, newProfileData.User2Orientation, 1);
        }
        if (newProfileData?.User1Province) {
            console.log("Provincia 1");
            await updateProvinceCount(newProfileData.User1Province, 1);
        }
        if (newProfileData?.User2Province) {
            console.log("Provincia 2");
            await updateProvinceCount(newProfileData.User2Province, 1);
        }
        return;
    }

    if (oldProfileData.User1Orientation !== newProfileData?.User1Orientation) {
        console.log("Es traba de nuevo 1");
        if (oldProfileData.User1Gender && oldProfileData.User1Orientation) {
            await updateOrientarionCount(oldProfileData.User1Gender, oldProfileData.User1Orientation, -1);
        }
        if (newProfileData?.User1Gender && newProfileData.User1Orientation) {
            await updateOrientarionCount(newProfileData.User1Gender, newProfileData.User1Orientation, 1);
        }
    }

    if (oldProfileData.User2Orientation !== newProfileData?.User2Orientation) {
        console.log("Es traba de nuevo 2");
        if (oldProfileData.User2Gender && oldProfileData.User2Orientation) {
            await updateOrientarionCount(oldProfileData.User2Gender, oldProfileData.User2Orientation, -1);
        }

        if (newProfileData?.User2Gender && newProfileData.User2Orientation) {
            await updateOrientarionCount(newProfileData.User2Gender, newProfileData.User2Orientation, 1);
        }
    }

    if (oldProfileData.User1Province !== newProfileData?.User1Province) {
        console.log("provincia 1 de nuevo");
        if (oldProfileData.User1Province) {
            await updateProvinceCount(oldProfileData.User1Province, -1);
        }
        if (newProfileData?.User1Province) {
            await updateProvinceCount(newProfileData.User1Province, 1);
        }
    }

    if (oldProfileData.User2Province !== newProfileData?.User2Province) {
        console.log("provincia 2 de nuevo");
        if (oldProfileData.User2Province) {
            await updateProvinceCount(oldProfileData.User2Province, -1);
        }
        if (newProfileData?.User2Province) {
            await updateProvinceCount(newProfileData.User2Province, 1);
        }
    }

    return;
};

