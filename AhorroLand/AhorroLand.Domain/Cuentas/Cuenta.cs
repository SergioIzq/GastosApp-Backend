using AhorroLand.Domain.Cuentas.Events;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.ValueObjects;
namespace AhorroLand.Domain.Cuentas;

public sealed class Cuenta : AbsEntity
{
    private Cuenta(Guid id, Nombre nombre, Cantidad saldo) : base(id)
    {
        Nombre = nombre;
        Saldo = saldo;
    }

    public Nombre Nombre { get; private set; }
    public Cantidad Saldo { get; private set; }

    public static Cuenta Create(Guid id, Nombre nombre, Cantidad saldo)
    {
        var cuenta = new Cuenta(id, nombre, saldo);

        cuenta.RaiseDomainEvent(new CuentaCreatedDomainEvent(cuenta.Id));

        return cuenta;
    }

    /// <summary>
    /// Deposita una cantidad en la cuenta, aumentando el saldo.
    /// </summary>
    public void Depositar(Cantidad cantidad)
    {
        Saldo = Saldo.Sumar(cantidad);
    }

    /// <summary>
    /// Retira una cantidad de la cuenta, disminuyendo el saldo, si hay fondos suficientes.
    /// </summary>
    public void Retirar(Cantidad cantidad)
    {
        if (Saldo.Valor < cantidad.Valor)
        {
            throw new InvalidOperationException(
                $"Saldo insuficiente en la cuenta {Nombre.Value} para retirar {cantidad.Valor}. Saldo actual: {Saldo.Valor}");
        }

        Saldo = Saldo.Restar(cantidad);
    }
}
