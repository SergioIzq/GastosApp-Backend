﻿using AppG.Entidades.BBDD;

namespace AppG.BBDD.Respuestas.Gastos
{
    public class GastoByIdRespuesta
    {
        public GastoByIdRespuesta()
        {
            GastoById = new Gasto();
            GastoRespuesta = new GastoRespuesta();
        }

        public Gasto GastoById { get; set; }
        public GastoRespuesta GastoRespuesta { get; set; }
    }
}
