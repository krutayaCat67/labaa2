using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using labaa2.Models;
using Microsoft.Win32;

namespace labaa2
{
    public partial class MainWindow : Window
    {
        private ClickGame? _game;
        private string _iconsFolder = "";
        private Enemy? _shownEnemy; // для какого противника сейчас загружена иконка
        private COrbController? _orbs;
        private DispatcherTimer? _timer;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!TryStartGame())
            {
                Close();
                return;
            }

            RefreshUi();
            StartIdleAnimation();
            StartGameLoop();
        }

        // Загружает шаблоны из JSON (Лаба 1) и создаёт игру.
        // Сначала ищет enemies.json и папку Icons рядом с .exe, иначе спрашивает через диалоги.
        private bool TryStartGame()
        {
            string jsonPath = Path.Combine(AppContext.BaseDirectory, "enemies.json");
            if (!File.Exists(jsonPath))
            {
                OpenFileDialog fileDialog = new OpenFileDialog
                {
                    Title = "Выберите JSON с шаблонами противников",
                    Filter = "JSON files (*.json)|*.json"
                };
                if (fileDialog.ShowDialog() != true)
                {
                    return false;
                }
                jsonPath = fileDialog.FileName;
            }

            _iconsFolder = Path.Combine(AppContext.BaseDirectory, "Icons");
            if (!Directory.Exists(_iconsFolder))
            {
                OpenFolderDialog folderDialog = new OpenFolderDialog
                {
                    Title = "Выберите папку с иконками противников"
                };
                if (folderDialog.ShowDialog() != true)
                {
                    return false;
                }
                _iconsFolder = folderDialog.FolderName;
            }

            CEnemyTemplateList templateList = new CEnemyTemplateList();
            templateList.LoadFromJson(jsonPath);

            if (templateList.GetAllEnemies().Count == 0)
            {
                MessageBox.Show("В файле нет шаблонов противников.");
                return false;
            }

            _game = new ClickGame(templateList.GetAllEnemies());
            return true;
        }

        private void EnemyImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_game == null || !_game.CanClick) return; // на перезарядке клик игнорируется

            BigNumber reward = _game.CurrentEnemy.Reward; // запоминаем до удара: после победы враг сменится

            bool defeated = _game.Click(); // всё решает игровой код, окно только вызывает метод

            FlashHit();
            if (defeated)
            {
                StatusText.Text = "TARGET ELIMINATED  +" + reward.ToShortString();
            }
            else
            {
                PlayHitAnimation();
            }

            RefreshUi();
        }

        // ---------- Лаба 3: таймер, перезарядка, бонусные сферы ----------

        private void StartGameLoop()
        {
            if (_game == null) return;

            _orbs = new COrbController(_game, new Size(OrbCanvas.ActualWidth, OrbCanvas.ActualHeight));

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_game == null || _orbs == null) return;

            _game.Update(0.1);
            _orbs.Update(0.1);
            SyncOrbSprites();
            RefreshUi();
        }

        private void SyncOrbSprites()
        {
            if (_orbs == null) return;

            OrbCanvas.Children.Clear();
            foreach (CCollectable orb in _orbs.Orbs)
            {
                OrbCanvas.Children.Add(orb.GetSprite());
            }
        }

        private void OrbCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _orbs?.SetSceneSize(e.NewSize);
        }

        // Tunneling-событие: сначала проверяем сферы; если попали — удар по противнику не происходит
        private void Portrait_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_orbs == null) return;

            if (_orbs.HandleClick(e.GetPosition(OrbCanvas)))
            {
                e.Handled = true;
                SyncOrbSprites();
                RefreshUi();
            }
        }

        private void CooldownUpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_game == null) return;

            _game.TryUpgradeCooldown();
            RefreshUi();
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_game == null) return;

            _game.TryUpgrade();
            RefreshUi();
        }

        private void RefreshUi()
        {
            if (_game == null) return;

            Player player = _game.Player;
            Enemy enemy = _game.CurrentEnemy;

            GoldText.Text = player.Gold.ToShortString();
            DamageText.Text = player.EffectiveDamage.ToShortString() + "  (lv " + player.DamageLevel + ")";

            EnemyNameText.Text = enemy.Name;
            RewardText.Text = enemy.Reward.ToShortString();

            HealthText.Text = enemy.CurrentHealth.ToShortString() + " / " + enemy.MaxHealth.ToShortString();

            // Процент здоровья считаем теми же операторами BigNumber: текущее * 100 / максимальное
            BigNumber percent = (enemy.CurrentHealth * new BigNumber(100)) / enemy.MaxHealth;
            int percentValue = int.Parse(percent.ToString());
            HealthBar.Value = percentValue;
            UpdateCondition(percentValue);

            UpgradeButton.Content = "UPGRADE DAMAGE  [" + player.UpgradeCost.ToShortString() + "]";
            UpgradeButton.IsEnabled = player.CanUpgrade;

            CooldownText.Text = player.Cooldown.Duration.ToString("0.00") + " s  (lv " + player.CooldownLevel + ")"
                + (player.Cooldown.IsReady ? "  READY" : "  ...");
            CooldownUpgradeButton.Content = "UPGRADE COOLDOWN  [" + player.CooldownUpgradeCost.ToShortString() + "]";
            CooldownUpgradeButton.IsEnabled = player.CanUpgradeCooldown;
            EffectsText.Text = player.ActiveEffectsCount > 0 ? "BONUS ACTIVE x" + player.ActiveEffectsCount : "";

            if (!ReferenceEquals(_shownEnemy, enemy))
            {
                ShowEnemyIcon(enemy);
                _shownEnemy = enemy;
            }
        }

        
        private void UpdateCondition(int percent)
        {
            string text;
            Color color;

            if (percent >= 60)
            {
                text = "FINE";
                color = Color.FromRgb(0x3F, 0xBF, 0x4F);
            }
            else if (percent >= 25)
            {
                text = "CAUTION";
                color = Color.FromRgb(0xE0, 0xA1, 0x00);
            }
            else
            {
                text = "DANGER";
                color = Color.FromRgb(0xC0, 0x20, 0x1A);
            }

            ConditionText.Text = text;
            ConditionText.Foreground = new SolidColorBrush(color);
            HealthBar.Foreground = new SolidColorBrush(color);
        }

        // Короткая красная вспышка поверх портрета
        private void FlashHit()
        {
            DoubleAnimation animation = new DoubleAnimation(0.45, 0.0, TimeSpan.FromMilliseconds(250));
            HitOverlay.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void ShowEnemyIcon(Enemy enemy)
        {
            string path = Path.Combine(_iconsFolder, enemy.IconName);
            EnemyImage.Source = File.Exists(path) ? new BitmapImage(new Uri(path)) : null;
            PlayAppearAnimation();
        }

        // ---------- Анимации ----------

        // Постоянное "дыхание": лёгкое покачивание вверх-вниз и пульсация размера
        private void StartIdleAnimation()
        {
            DoubleAnimation bob = new DoubleAnimation(0, -8, TimeSpan.FromSeconds(1.2))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            MoveTransform.BeginAnimation(TranslateTransform.YProperty, bob);

            DoubleAnimation breathe = new DoubleAnimation(1.0, 1.04, TimeSpan.FromSeconds(1.5))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            BreathTransform.BeginAnimation(ScaleTransform.ScaleXProperty, breathe);
            BreathTransform.BeginAnimation(ScaleTransform.ScaleYProperty, breathe);
        }

        // Реакция на удар: тряска, наклон и короткое сжатие
        private void PlayHitAnimation()
        {
            MoveTransform.BeginAnimation(TranslateTransform.XProperty,
                MakeKeyFrames((0, 0), (40, -14), (90, 12), (140, -8), (190, 4), (240, 0)));

            TiltTransform.BeginAnimation(RotateTransform.AngleProperty,
                MakeKeyFrames((0, 0), (40, -4), (100, 3), (170, -1.5), (240, 0)));

            DoubleAnimationUsingKeyFrames squish = MakeKeyFrames((0, 1), (60, 0.9), (200, 1));
            PopTransform.BeginAnimation(ScaleTransform.ScaleXProperty, squish);
            PopTransform.BeginAnimation(ScaleTransform.ScaleYProperty, squish);
        }

        // Появление нового противника: проявление и "выпрыгивание"
        private void PlayAppearAnimation()
        {
            EnemyImage.BeginAnimation(UIElement.OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300)));

            DoubleAnimation pop = new DoubleAnimation(0.6, 1.0, TimeSpan.FromMilliseconds(350))
            {
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.4 }
            };
            PopTransform.BeginAnimation(ScaleTransform.ScaleXProperty, pop);
            PopTransform.BeginAnimation(ScaleTransform.ScaleYProperty, pop);
        }

        // Вспомогательный метод: анимация по списку (миллисекунды, значение)
        private static DoubleAnimationUsingKeyFrames MakeKeyFrames(params (int ms, double value)[] frames)
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            foreach ((int ms, double value) in frames)
            {
                animation.KeyFrames.Add(new LinearDoubleKeyFrame(
                    value, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(ms))));
            }
            return animation;
        }
    }
}