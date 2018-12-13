using Perpetuum.ExportedTypes;
using Perpetuum.Items;
using Perpetuum.Zones.Effects;

namespace Perpetuum.Modules.EffectModules
{
    public class TargetPainterModule : EffectModule
    {
        private readonly ItemProperty _effectStealthStrengthModifier;
        private readonly ItemProperty _effectSurfaceAreaModifier;

        public TargetPainterModule() : base(true)
        {
            _effectStealthStrengthModifier = new ModuleProperty(this, AggregateField.effect_stealth_strength_modifier);
            _effectSurfaceAreaModifier = new ModuleProperty(this, AggregateField.effect_signature_radius_modifier);
            AddProperty(_effectStealthStrengthModifier);
            AddProperty(_effectSurfaceAreaModifier);
        }

        protected override void SetupEffect(EffectBuilder effectBuilder)
        {
            effectBuilder.SetType(EffectType.effect_target_painting)
                                .SetSource(ParentRobot)
                                .WithPropertyModifier(_effectStealthStrengthModifier.ToPropertyModifier())
                                .WithPropertyModifier(_effectSurfaceAreaModifier.ToPropertyModifier());
        }
    }
}