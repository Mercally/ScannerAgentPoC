import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import './SingleScan.css';

function SingleScan() {

    const [scanners, setScanners] = useState([]);
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);


    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:5001/scannerHub')
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {

        if (!connection)
            return;

        connection.start().then(() => {
            console.log("Conectado a SignalR");

            connection.on("DocumentScanned", (data: ScanSingleImage) => {
                console.log(data);
            });



        }).catch((err) => console.error("Error al conectar a SignalR:", err));

    }, [connection]);

    const triggerSingleScan = () => {

        if (!connection)
            return;

        connection.invoke("ScanSingleImage");
            //.then((data:ScanSingleImage) => console.log(data))
            //.catch((err) => console.error("Error obteniendo scanners:", err));

    }

    return (
        <div>
            <h1>Escanear una imagen</h1>
            <button onClick={() => triggerSingleScan()}>Escanear</button>
        </div>
    );
}

interface ScanSingleImage {
    tempFileId: string;
    base64Data: string;
}

export default SingleScan;