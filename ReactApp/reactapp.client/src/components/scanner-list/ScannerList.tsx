import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import ReactFilePreviewer from 'react-file-previewer';

import './ScannerList.css';


function ScannerList() {

    const [scanners, setScanners] = useState<string[]>([]);
    const [selectedScanner, setSelectedScanner] = useState<string>('');
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [scannedImage, setScannedImage] = useState<ScanDocumentResult | null>(null);

    const [documentId, setDocumentId] = useState<string | null>(null);

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

        setIsLoading(true);

        connection.start().then(() => {
            console.log("Conectado a SignalR");

            connection.invoke("GetScanners")
                .then((scanners: string[]) => {
                    setScanners(scanners);
                    setSelectedScanner(scanners[0]);
                    setIsLoading(false);
                })
                .catch((err) => console.error("Error obteniendo scanners:", err));

            connection.on("DocumentScanned", (data: ScanDocumentResult) => {
                console.log(data);
                setScannedImage(data);
            });

            connection.on("NewPageScanned", (data: ScanDocumentResult) => {
                console.log(data);
                setScannedImage(data);
            });

            connection.on("DocumentUploaded", () => {
                console.log("Document uploaded successfully");

                setScannedImage(null);
                setDocumentId(null);
                setIsLoading(false);
            })

        }).catch((err) => console.error("Error al conectar a SignalR:", err));

    }, [connection]);

    const triggerSingleScan = () => {

        if (!connection)
            return;

        connection.invoke("ScanSingleImage", selectedScanner)
            .then((data:string) => console.log(data))
            .catch((err) => console.error("Error lanzando scanner:", err));

    }

    const triggerDocumentScan = () => {

        if (!connection)
            return;

        setDocumentId(null);
        const guid = generateGuid();

        connection.invoke("ScanDocument", selectedScanner, guid)
            .then((data: string) => {
                console.log(data);
                setDocumentId(guid);
            })
            .catch((err) => console.error("Error lanzando scanner:", err));
    }

    const triggerDocumentScanNewPage = () => {

        if (!connection)
            return;

        if (!documentId)
            return;

        connection.invoke("ScanDocument", selectedScanner, documentId)
            .then((data: string) => console.log(data))
            .catch((err) => console.error("Error lanzando scanner:", err));
    }

    const triggerDocumentStop = () => {

        if (!connection)
            return;

        if (!documentId)
            return;

        connection.invoke("StopDocumentScan", documentId)
            .then((data: string) => {
                console.log(data);
                setIsLoading(true);
            })
            .catch((err) => {
                console.error("Error lanzando scanner:", err);
                setIsLoading(false);
            });
    }

    return (
        <div>
            <h1>Escaneres Disponibles</h1>
            <select onChange={(e) => setSelectedScanner(e.target.value)}>
                {
                    scanners.map((scanner, index) => (
                        <option key={index} value={scanner}>{scanner}</option>
                    ))
                }
            </select>
            <br />
            <div>
                <h3>Escanear una imagen</h3>
                <button
                    disabled={isLoading}
                    onClick={() => triggerSingleScan()}>Escanear imagen</button>
            </div>
            <br />
            <div>
                <h3>Escanear un documento</h3>
                <button
                    disabled={isLoading && !documentId}
                    onClick={() => triggerDocumentScan()}>Escanear nuevo documento</button>

                <button
                    disabled={isLoading || documentId == null}
                    onClick={() => triggerDocumentScanNewPage()}>Escanear otra pagina</button>

                <button
                    disabled={isLoading || documentId == null}
                    onClick={() => triggerDocumentStop()}>Terminar documento</button>
            </div>
            <div>
                <h3>Previewer</h3>
                <div>
                    {scannedImage?.base64Data != null && (
                        <ReactFilePreviewer
                            file={{
                                data: scannedImage.base64Data,
                                mimeType: "application/pdf",
                                name: 'test.pdf'
                            }}
                        />
                    )}
                </div>
            </div>
        </div>
  );
}

interface ScanDocumentResult {
    tempFolderId: string;
    tempPageId: string;
    base64Data: string;
    fileName: string;
}

const generateGuid = () => {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}



export default ScannerList;