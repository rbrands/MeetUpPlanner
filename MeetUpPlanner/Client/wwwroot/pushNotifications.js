(function () {
    // Note: Replace with your own key pair before deploying
    const applicationServerPublicKey = 'BJkgu1ZbFHQm1gQCkYBvsHgZn8-f_v9f9HzIi9UQlCYq2DfUzv4OEx1Dfg9gD0s88fSQ8Ya8kdE4Ib422JHk_E0';

    window.blazorPushNotifications = {
        requestSubscription: async () => {
            const worker = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await worker.pushManager.getSubscription();
            if (!existingSubscription) {
                const newSubscription = await subscribe(worker);
                if (newSubscription) {
                    return {
                        url: newSubscription.endpoint,
                        p256dh: arrayBufferToBase64(newSubscription.getKey('p256dh')),
                        auth: arrayBufferToBase64(newSubscription.getKey('auth'))
                    };
                }
            }
            else {
                return {
                    url: existingSubscription.endpoint,
                    p256dh: arrayBufferToBase64(existingSubscription.getKey('p256dh')),
                    auth: arrayBufferToBase64(existingSubscription.getKey('auth'))
                };
            }
        },
        getExistingSubscription: async () => {
            const worker = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await worker.pushManager.getSubscription();
            if (existingSubscription) {
                return {
                    url: existingSubscription.endpoint,
                    p256dh: arrayBufferToBase64(existingSubscription.getKey('p256dh')),
                    auth: arrayBufferToBase64(existingSubscription.getKey('auth'))
                };
            }
        }
    };

    async function subscribe(worker) {
        try {
            return await worker.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerPublicKey
            });
        } catch (error) {
            if (error.name === 'NotAllowedError') {
                return null;
            }
            throw error;
        }
    }

    function arrayBufferToBase64(buffer) {
        // https://stackoverflow.com/a/9458996
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
})();
