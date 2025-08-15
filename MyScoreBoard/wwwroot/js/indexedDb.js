// Simple IndexedDB helper for MyScoreBoard
// Database: myscoreboard, Store: games (key auto-increment)

const DB_NAME = 'myscoreboard';
const DB_VERSION = 7; // bump to force schema upgrade
const STORE_GAMES = 'games';
const STORE_ACTIVE = 'active';

export function initDb() {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);

    request.onupgradeneeded = (event) => {
      const db = request.result;
      if (!db.objectStoreNames.contains(STORE_GAMES)) {
        // Use auto-increment key without keyPath for games store
        const store = db.createObjectStore(STORE_GAMES, { autoIncrement: true });
        store.createIndex('SessionId', 'SessionId', { unique: true });
        store.createIndex('StartedUtc', 'StartedUtc', { unique: false });
      }
      // Always (re)create ACTIVE store with out-of-line keys to avoid keyPath mismatch
      if (db.objectStoreNames.contains(STORE_ACTIVE)) {
        db.deleteObjectStore(STORE_ACTIVE);
      }
      // Simple store without keyPath (no autoIncrement): we will supply a fixed key 'current'
      db.createObjectStore(STORE_ACTIVE);
    };

    request.onsuccess = () => {
      request.result.close();
      resolve();
    };

    request.onerror = () => reject(request.error);
  });
}

export function addItem(storeName, value) {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);
    request.onsuccess = () => {
      const db = request.result;
      const tx = db.transaction(storeName, 'readwrite');
      const store = tx.objectStore(storeName);
      const addReq = store.add(value);
      addReq.onsuccess = () => {
        const key = addReq.result;
        tx.oncomplete = () => {
          db.close();
          resolve(key);
        };
      };
      addReq.onerror = () => reject(addReq.error);
    };
    request.onerror = () => reject(request.error);
  });
}

export function getAll(storeName) {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);
    request.onsuccess = () => {
      const db = request.result;
      const tx = db.transaction(storeName, 'readonly');
      const store = tx.objectStore(storeName);
      
      // Use cursor to get both keys and values
      const items = [];
      const cursorReq = store.openCursor();
      
      cursorReq.onsuccess = (event) => {
        const cursor = event.target.result;
        if (cursor) {
          const item = cursor.value;
          item.Key = cursor.key; // Add the auto-generated key to the object
          items.push(item);
          cursor.continue();
        } else {
          // All items processed
          tx.oncomplete = () => {
            db.close();
            resolve(items);
          };
        }
      };
      
      cursorReq.onerror = () => reject(cursorReq.error);
    };
    request.onerror = () => reject(request.error);
  });
}

export function deleteItem(storeName, key) {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);
    request.onsuccess = () => {
      const db = request.result;
      const tx = db.transaction(storeName, 'readwrite');
      const store = tx.objectStore(storeName);
      const delReq = store.delete(key);
      delReq.onsuccess = () => {
        tx.oncomplete = () => {
          db.close();
          resolve();
        };
      };
      delReq.onerror = () => reject(delReq.error);
    };
    request.onerror = () => reject(request.error);
  });
}

export function putItem(storeName, value) {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);
    request.onsuccess = () => {
      const db = request.result;
      const tx = db.transaction(storeName, 'readwrite');
      const store = tx.objectStore(storeName);
      
      if (storeName === STORE_ACTIVE) {
        // For active store, use put with explicit key
        const putReq = store.put(value, 'current');
        putReq.onsuccess = () => {
          tx.oncomplete = () => {
            db.close();
            resolve('current');
          };
        };
        putReq.onerror = () => reject(putReq.error);
      } else {
        // For games store, use put without key (auto-increment)
        const putReq = store.put(value);
        putReq.onsuccess = () => {
          const resultKey = putReq.result;
          tx.oncomplete = () => {
            db.close();
            resolve(resultKey);
          };
        };
        putReq.onerror = () => reject(putReq.error);
      }
    };
    request.onerror = () => reject(request.error);
  });
}

export function getFirst(storeName) {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);
    request.onsuccess = () => {
      const db = request.result;
      const tx = db.transaction(storeName, 'readonly');
      const store = tx.objectStore(storeName);
      
      if (storeName === STORE_ACTIVE) {
        // For active store, get the specific 'current' key
        const getReq = store.get('current');
        getReq.onsuccess = () => {
          tx.oncomplete = () => {
            db.close();
            resolve(getReq.result || null);
          };
        };
        getReq.onerror = () => reject(getReq.error);
      } else {
        // For other stores, get all and return first
        const getAllReq = store.getAll();
        getAllReq.onsuccess = () => {
          const items = getAllReq.result || [];
          tx.oncomplete = () => {
            db.close();
            resolve(items.length > 0 ? items[0] : null);
          };
        };
        getAllReq.onerror = () => reject(getAllReq.error);
      }
    };
    request.onerror = () => reject(request.error);
  });
}

export function clearStore(storeName) {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION);
    request.onsuccess = () => {
      const db = request.result;
      const tx = db.transaction(storeName, 'readwrite');
      const store = tx.objectStore(storeName);
      
      if (storeName === STORE_ACTIVE) {
        // For active store, delete the specific 'current' key
        const deleteReq = store.delete('current');
        deleteReq.onsuccess = () => {
          tx.oncomplete = () => {
            db.close();
            resolve();
          };
        };
        deleteReq.onerror = () => {
          // If delete fails, still resolve (item might not exist)
          tx.oncomplete = () => {
            db.close();
            resolve();
          };
        };
      } else {
        // For other stores, clear everything
        const clearReq = store.clear();
        clearReq.onsuccess = () => {
          tx.oncomplete = () => {
            db.close();
            resolve();
          };
        };
        clearReq.onerror = () => reject(clearReq.error);
      }
    };
    request.onerror = () => reject(request.error);
  });
}
