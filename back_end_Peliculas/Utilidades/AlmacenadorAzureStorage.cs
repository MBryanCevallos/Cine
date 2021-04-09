﻿using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_Peliculas.Utilidades
{
    public class AlmacenadorAzureStorage
    {
        private string connectionString;
        public AlmacenadorAzureStorage(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage"); // para comunicarnos con la instancia de azure storage
        }

        //public async Task<string> GuardarArchivos(string contenedor, IFormFile archivo) // Task<String> xq retornar la url
        //{
        //    var cliente = new BlobContainerClient(connectionString, contenedor);
        //    await cliente.CreateIfNotExistsAsync(); //crea el contedor en caso de no existir por ejemplo la primera vez
        //    cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob); // publico a nivel de blob

        //    var extension = Path.GetExtension(archivo.FileName);
        //    var archivoNombre = $"{Guid.NewGuid()}{extension}"; // un Guid para casa imagen es decir no tendremos nombres repetidos
        //    var blob = cliente.GetBlobClient(archivoNombre);
        //    await blob.UploadAsync(archivo.OpenReadStream());
        //    return blob.Uri.ToString();
        //}
    }
}
