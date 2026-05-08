import { getProfileByCurrentProvince  } from "./database";
// import { GeoPoint } from '@google-cloud/firestore';
import { ProfileData } from "./shared";
import { calcularEdad } from "./shared";
// import { geohashQueryBounds, /*distanceBetween*/ } from "geofire-common";


/*
 * Searches for profiles within a given radius of a center point, optionally filtered by age and gender.
 */
export async function buscarPerfilesCercanosModule( req : any, res: any ) : Promise<void>  {
    try {
        // const authHeader = req.headers.authorization;
        // if (!authHeader?.startsWith('Bearer ')) {
        //     res.status(403).json({ error: 'Falta token' });
        // }

        // const token = authHeader?.split('Bearer ')[1];
        // try {
        //     const client = auth; // Usamos 'auth' en lugar de 'client' que no estaba definido
        //     await client.verifyIdToken(token!); // Usar verifyIdToken para tokens de Firebase Auth
            // const expectedAudience = '104622036438253782219'; // TODO:  Move to environment variable

            // La verificación de 'aud' no es estándar para tokens de Firebase Auth
            // Considera verificar 'uid' o usar las reglas de seguridad de Firebase
            // if (info.aud !== expectedAudience) {
            //     res.status(403).json({ error: 'Token inválido' });
            //     return;
            // }
        // } catch (error) {
        //     console.error("Error verifying token:", error);
        //     res.status(403).json({ error: 'Token inválido' });
        //     // return; // Importante agregar 'return'
        // }

        const { center, /*radiusInKm,*/ filtros  = {} } = req.body;
        const centerPoint: [number, number] = [center["latitude"], center["longitude"]];
        // const bounds = geohashQueryBounds(centerPoint, radiusInKm);
        // const promises: Promise<FirebaseFirestore.QuerySnapshot<FirebaseFirestore.DocumentData>>[] = [];
        // for (const b of bounds) {
        //    const {q1, q2} = await updateProfileGeoHash(b);
            
        //     promises.push(q1.get(), q2.get());
        // }

        const provinciaCentro = await obtenerProvinciaDesdeCoordenadas(centerPoint[0], centerPoint[1]);
        const centroProv = (provinciaCentro as string).toLowerCase();
        const {q1, q2} = await getProfileByCurrentProvince(centroProv);

        const [snap1, snap2] = await Promise.all([q1.get(), q2.get()]);

        const idsVistos = new Set<string>();
        let allUniqueDocs: ({ id: string } & ProfileData)[] = [];

        snap1.docs.forEach((doc : any) => {
            idsVistos.add(doc.id); // Agregamos el ID al set
            allUniqueDocs.push({ id: doc.id, ...(doc.data() as ProfileData) });
        });

        snap2.docs.forEach((doc : any) => {
                if (!idsVistos.has(doc.id)) { // <-- VERIFICACIÓN DE DUPLICADOS
                    idsVistos.add(doc.id);
                    allUniqueDocs.push({ id: doc.id, ...(doc.data() as ProfileData) });
                }
            });

        const matchingDocs: ({ id: string } & ProfileData)[] = [];

        for (const dataWithId of allUniqueDocs) {
            const data = dataWithId as ProfileData;

                if(!data.User1GeoPoint && !data.User2GeoPoint) {
                    continue;
                }
                // const locations = [
                //     data.User1GeoPoint,
                //     data.User2GeoPoint
                // ].filter((loc): loc is GeoPoint => loc !== undefined || loc !== null);



                // let match = false;
                // for (const loc of locations) {
                //     if(loc.latitude === undefined || loc.longitude === undefined) {
                //         continue;
                //     }
                //     console.log("latitud:"+ centerPoint[0]+", longitud: "+centerPoint[1])
                //     console.log("provinciaCentro pone: "+provinciaCentro)
                //     if(provinciaCentro){

                //         match = true;
                //         break;
                //     }
                //     // Asegurate de que 'distanceBetween' este correctamente importado y funcionando con GeoPoint
                //     // const dist = distanceBetween([loc.latitude, loc.longitude], centerPoint);
                //     // if (dist <= radiusInKm) {
                //     //     match = true;
                //     //     break;
                //     // }

                // }
                // if (!match) continue;

                let cumpleFiltros = true;

                // filtros por edad
                if (filtros.edad && filtros.edad.length === 2) {
                    let cumple = false;

                    const min = filtros.edad[0];
                    const max = filtros.edad[1];
                    const edad1 = data.User1BirthDate ? calcularEdad(data.User1BirthDate) : null;
                    const edad2 = data.User2BirthDate ? calcularEdad(data.User2BirthDate) : null;

                    cumple = [edad1, edad2].some(edad => edad !== null && edad >= min && edad <= max);

                    cumpleFiltros = cumpleFiltros && cumple;
                }
                
                // filtros por interes
                if(filtros.interes) {
                    let cumpleFiltrosInteres = false;
                    if(filtros.interes==="Todos"){
                            cumpleFiltrosInteres=true 
                    }else{
                        cumpleFiltrosInteres = data.Interest === filtros.interes;
                    }
                    cumpleFiltros = cumpleFiltros && cumpleFiltrosInteres;
                }

                if (filtros.sexo) {
                    let cumple = false; 

                    const g1 = data.User1Gender;
                    const g2 = data.User2Gender;
                    const multiSex = filtros.sexo;
                    const sexos = multiSex.split(",");

                    for (const sexo of sexos ) {
                        switch (sexo) {
                            case 'Todos':
                                cumple = true;
                                break;
                            case 'Hombre - Mujer':
                                cumple = (g1 === 'Hombre' && g2 === 'Mujer') || (g1 === 'Mujer' && g2 === 'Hombre');
                                break;
                            case 'Hombre - Hombre':
                                cumple = g1 === 'Hombre' && g2 === 'Hombre';
                                break;
                            case 'Mujer - Mujer':
                                cumple = g1 === 'Mujer' && g2 === 'Mujer';
                                break;
                            case 'Hombre':
                                cumple = g1 === 'Hombre' || g2 === 'Hombre';
                                break;
                            case 'Mujer':
                                cumple = g1 === 'Mujer' || g2 === 'Mujer';
                                break;
                        }

                        if(cumple) {
                            break;
                        }
                    }
                    
                    cumpleFiltros = cumpleFiltros && cumple;
                }

                if(filtros.education) {
                    let cumple = false;
                    if(filtros.education==="Todos"){
                        cumple=true;
                    }else{
                        if(data.User1EducationLevel === filtros.education || data.User2EducationLevel === filtros.education) {
                            cumple = true;
                        }
                    }
                    

                    cumpleFiltros = cumpleFiltros && cumple;
                }

                if(filtros.rasgos) {
                    let cumple = false;
                    if(filtros.rasgos ==="Todos"){
                        cumple=true;
                    }else{
                        if(data.User1Traits === filtros.rasgos || data.User2Traits === filtros.rasgos) {
                            cumple = true;
                        }
                    }
                    cumpleFiltros = cumpleFiltros && cumple;
                }

                if(filtros.orientacion) {
                    let cumple = false;
                    if(filtros.orientacion ==="Todos"){
                        cumple=true;
                    }else{
                        if(data.User1Orientation === filtros.orientacion || data.User2Orientation === filtros.orientacion) {
                            cumple = true;
                        }
                    }

                    cumpleFiltros = cumpleFiltros && cumple;
                }

                if(filtros.cabello) {
                    let cumple = false;
                    if(filtros.cabello ==="Todos"){
                        cumple=true;
                    }else{
                        if(data.User1HairType === filtros.cabello || data.User2HairType === filtros.cabello) {
                            cumple = true;
                        }
                    }
                    cumpleFiltros = cumpleFiltros && cumple;
                }

                if(filtros.ojos) {
                    let cumple = false;
                    if(filtros.ojos ==="Todos"){
                        cumple=true;
                    }else{
                        if(data.User1EyeColor === filtros.ojos || data.User2EyeColor === filtros.ojos) {
                            cumple = true;
                        }
                    }
                    cumpleFiltros = cumpleFiltros && cumple;
                }

                // if (provinciaCentro) {
                //     const provincia1 = data.User1CurrentProvince ? data.User1CurrentProvince.toLowerCase() : "";
                //     const provincia2 = data.User2CurrentProvince ? data.User2CurrentProvince.toLowerCase() : "";
                //    const centroProv = (provinciaCentro as string).toLowerCase();

                //     const coincideProvincia = provincia1 === centroProv || provincia2 === centroProv;
                //     if (!coincideProvincia) continue; // ❌ Si no coincide, descartar el perfil
                // }

                console.log('ACA finalizar filtro de sexo : ', cumpleFiltros);
                if (cumpleFiltros) {
                    console.log('ACA Ver si cumple los filtros: ', cumpleFiltros);
                    matchingDocs.push({ id: dataWithId.id, ...data });
                }
            
        }
        res.json(matchingDocs);
    } catch (error: any) {
        console.error("❌ Error:", error);
        res.status(500).json({ error: error.message });
    }
};

export async function obtenerProvinciaDesdeCoordenadas(lat: number, lng: number): Promise<string | null> {
    const apiKey = "AIzaSyBFe9CIf1VI0TDYGIF-lGwbjPFxkupxVHY"; // poné tu key en variables de entorno
   const url = `https://maps.googleapis.com/maps/api/geocode/json?latlng=${lat},${lng}&key=${apiKey}`;
   const response = await fetch(url);
   const data = await response.json();
   console.log("data es :"+data.results)

  if (data.status !== "OK" || !data.results.length) return null;

  for (const result of data.results) {

    const provincia = result.address_components.find((c: any) =>
      c.types.includes("administrative_area_level_1")
    );
    if (provincia) {
        console.log("Provincia: "+provincia)
      return provincia.long_name; // Ejemplo: "Chaco"
    }
  }
    console.log("saque de merca, bueno nose que hacer aca")
  return null;
}

/*function aplicarFiltrosAdicionales(data: ProfileData, filtros: any): boolean {
    let cumpleFiltros = true;
    
    // --- 1. Filtro por Edad ---
    if (filtros.edad && filtros.edad.length === 2) {
        let cumple = false;
        const [min, max] = filtros.edad;
        
        const edad1 = data.User1BirthDate ? calcularEdad(data.User1BirthDate) : null;
        const edad2 = data.User2BirthDate ? calcularEdad(data.User2BirthDate) : null;

        cumple = [edad1, edad2].some(edad => edad !== null && (edad as number) >= min && (edad as number) <= max);
        cumpleFiltros = cumpleFiltros && cumple;
    }

    // --- 2. Filtro por Interés ---
    if(cumpleFiltros && filtros.interes) {
        let cumple = false;
        if(filtros.interes === "Todos"){
            cumple = true;
        } else {
            cumple = data.Interest === filtros.interes;
        }
        cumpleFiltros = cumpleFiltros && cumple;
    }
    
    // --- 3. Filtro por Sexo ---
    if (cumpleFiltros && filtros.sexo) {
        let cumple = false; 
        const g1 = data.User1Gender;
        const g2 = data.User2Gender;
        const sexos = filtros.sexo.split(",");

        for (const sexo of sexos ) {
            switch (sexo.trim()) {
                case 'Todos':
                    cumple = true;
                    break;
                case 'Hombre - Mujer':
                    cumple = (g1 === 'Hombre' && g2 === 'Mujer') || (g1 === 'Mujer' && g2 === 'Hombre');
                    break;
                case 'Hombre - Hombre':
                    cumple = g1 === 'Hombre' && g2 === 'Hombre';
                    break;
                case 'Mujer - Mujer':
                    cumple = g1 === 'Mujer' && g2 === 'Mujer';
                    break;
                case 'Hombre':
                    cumple = g1 === 'Hombre' || g2 === 'Hombre';
                    break;
                case 'Mujer':
                    cumple = g1 === 'Mujer' || g2 === 'Mujer';
                    break;
            }
            if(cumple) break;
        }
        cumpleFiltros = cumpleFiltros && cumple;
    }

    // --- 4. Filtro por Nivel Educativo ---
    if(cumpleFiltros && filtros.education) {
        let cumple = (filtros.education === "Todos") || 
                     (data.User1EducationLevel === filtros.education || data.User2EducationLevel === filtros.education);
        cumpleFiltros = cumpleFiltros && cumple;
    }

    // --- 5. Filtro por Rasgos ---
    if(cumpleFiltros && filtros.rasgos) {
        let cumple = (filtros.rasgos === "Todos") || 
                     (data.User1Traits === filtros.rasgos || data.User2Traits === filtros.rasgos);
        cumpleFiltros = cumpleFiltros && cumple;
    }

    // --- 6. Filtro por Orientación ---
    if(cumpleFiltros && filtros.orientacion) {
        let cumple = (filtros.orientacion === "Todos") || 
                     (data.User1Orientation === filtros.orientacion || data.User2Orientation === filtros.orientacion);
        cumpleFiltros = cumpleFiltros && cumple;
    }

    // --- 7. Filtro por Cabello ---
    if(cumpleFiltros && filtros.cabello) {
        let cumple = (filtros.cabello === "Todos") || 
                     (data.User1HairType === filtros.cabello || data.User2HairType === filtros.cabello);
        cumpleFiltros = cumpleFiltros && cumple;
    }

    // --- 8. Filtro por Ojos ---
    if(cumpleFiltros && filtros.ojos) {
        let cumple = (filtros.ojos === "Todos") || 
                     (data.User1EyeColor === filtros.ojos || data.User2EyeColor === filtros.ojos);
        cumpleFiltros = cumpleFiltros && cumple;
    }

    return cumpleFiltros;
}*/