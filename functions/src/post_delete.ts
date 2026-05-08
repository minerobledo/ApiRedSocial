import { getContestsSnap, getBatchDB, getPosts } from './database';

export async function postDelete(event : any): Promise<void> {
    const postId = event.params.postId;
    await actualizarConcursosPorEliminacionDePost(postId);
};

async function actualizarConcursosPorEliminacionDePost(postId: string) {
    const contestsSnap = await getContestsSnap();
  
    const batch = await getBatchDB();
  
    for (const doc of contestsSnap.docs) {
      const contest = doc.data();
      const ref = doc.ref;
  
      const postIds: string[] = contest.PostIds || [];
      const winnerPostId: string = contest.WinnerPostId;
  
      const contienePost = postIds.includes(postId);
      const esGanador = winnerPostId === postId;
  
      if (!contienePost && !esGanador) continue; // no afecta este concurso
  
      // 1. Remover post del array si está
      const nuevosPostIds = postIds.filter((id) => id !== postId);
      const nuevoGanador = esGanador ? await buscarNuevoGanador(nuevosPostIds) : winnerPostId;
  
      batch.update(ref, {
        PostIds: nuevosPostIds,
        WinnerPostId: nuevoGanador || null,
      });
    }
  
    await batch.commit();
  }

  // Encuentra el post con más LikesCount entre una lista de IDs
  async function buscarNuevoGanador(postIds: string[]): Promise<string | null> {
    if (postIds.length === 0) return null;
  
    let mejorPostId = null;
    let maxLikes = -1;
  
    const postSnaps = await Promise.all(
      postIds.map((id) => getPosts(id))
    );
  
    for (const snap of postSnaps) {
      if (!snap.exists) continue;
      const data = snap.data();
      const likes = data?.LikesCount ?? 0;
  
      if (likes > maxLikes) {
        maxLikes = likes;
        mejorPostId = snap.id;
      }
    }
  
    return mejorPostId;
  }