import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import './ScannerList.css';

function ScannerList() {

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

            connection.invoke("GetScanners")
                .then((data) => setScanners(data))
                .catch((err) => console.error("Error obteniendo scanners:", err));

            //connection.on("DocumentScanned", (filePath) => {
                
            //})

        }).catch((err) => console.error("Error al conectar a SignalR:", err));

    }, [connection]);

    return (
        <div>
            <h1>Escaneres Disponibles</h1>
            <select>
                {
                    scanners.map((scanner, index) => (
                        <option key={index} value={scanner}>{scanner}</option>
                    ))
                }
            </select>
        </div>
  );
}

export default ScannerList;