﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TP0.Helpers
{
    public class AdaptadorApple : DispositivoInteligente
    {
        public AdaptadorApple(string nom, string idnuevo, double kWxHoraNuevo, double mx, double mn)
        {
            KWxHora = kWxHoraNuevo;
            Nombre = nom;
            Codigo = idnuevo;
            Max = mx;
            Min = mn;
            estadosAnteriores = new List<State>();
            ConsumoAcumulado = 0;
            EsInteligente = true;
            //act = new Actuador(Int32.Parse(idnuevo));
        }
        public AdaptadorApple()
        {

        }
        //private Marca_Apple apple;
        Marca_Apple apple = new Marca_Apple();

        public override void ActualizarUltimoEstado()
        {
            apple.ActualizarUltimoEstadoAPPLE(this);
        }
        public override State GetEstado()
        {
            return apple.GetEstadoAPPLE(this);
        }
        public override List<State> GetEstados()
        {
            return apple.GetEstadosAPPLE(this);
        }
        public override bool EstaEncendido()
        {
            return apple.EstaEncendidoAPPLE(this);
        }
        public override bool EstaApagado()
        {
            return apple.EstaApagadoAPPLE(this);
        }
        public override bool EnAhorro()
        {
            return apple.EnAhorroAPPLE(this);
        }
        public override void Encender()
        {
            apple.EncenderAPPLE(this);
        }
        public override void Apagar()
        {
            apple.ApagarAPPLE(this);
        }
        public override void AhorrarEnergia()
        {
            apple.AhorrarEnergiaAPPLE(this);
        }
        public override void ActualizarConsumoAcumulado(string FechaAlta)
        {
            apple.ActualizarConsumoAcumuladoAPPLE(FechaAlta, this);
        }
        public override double Consumo()
        {
            return apple.ConsumoAPPLE(this);
        }
        public override double ConsumoActual()
        {
            return apple.ConsumoActualAPPLE(this);
        }
        public override double ConsumoEnHoras(double horas)
        {
            return apple.ConsumoEnHorasAPPLE(horas, this);
        }
        public override double ConsumoEnPeriodo(DateTime fInicial, DateTime fFinal)
        {
            return apple.ConsumoEnPeriodoAPPLE(fInicial, fFinal, this);
        }
        public override void AgregarEstado(State est)
        {
            apple.AgregarEstadoAPPLE(est, this);
        }
    }

}