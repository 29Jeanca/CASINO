using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using NAudio.Wave;
namespace CASINO.Clases.Extras
{
    public static class ControladorSonido
    {
        public static void ReproducirAudio(string rutaAudio)
        {
            var archivoAudio = new AudioFileReader(rutaAudio);

            var salidaDispositivo = new WaveOutEvent();

            salidaDispositivo.Init(archivoAudio);

            salidaDispositivo.Play();


        }
    }
}
