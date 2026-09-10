# Apple Grapple Replica — Proje Devir Teslim Dokümanı

> Bu doküman, Loop Games case projesinin şu ana kadar yapılan kısmını, mimari kararları ve
> **geri kalanını nasıl tamamlayacağını** adım adım anlatır. Token/süre kısıtı nedeniyle
> geliştirmeye kendi başına devam edeceksin — bu dosya senin yol haritan.

---

## 1. Case'in özeti (ne yapılacaktı?)

Kaynak: [Assets/Template/AppleGrapple.txt](Assets/Template/AppleGrapple.txt), "Loop Games – Senior Game Developer Case.pdf"

- Unity 6 (LTS), top-down bir arena dövüş oyunu ("Apple Grapple" — isimdeki "grapple" bir kanca/çekme mekaniği DEĞİL, sadece oyunun adı).
- 1 oyuncu + 3 AI karakter (toplam 4), son ayakta kalan kazanır.
- Yerde rastgele aralıklarla kılıç spawn olur, karakterler üzerine yürüyerek toplar.
- **Karakterler aynı anda birden fazla kılıç taşıyabilir** (etraflarında dönen kılıçlar — referans görsellerde net).
- İki kılıç çarpışırsa ikisi de yok olur.
- Kılıçsız (silahsız) karakter 3 vuruşta ölür.
- Sadece kılıç dövüşü var — hız artırma, ateşli silah yok.
- AI: ne çok aptal ne çok zeki — en yakın kılıca gider, tehlikeden kaçar, menzile giren düşmana saldırır.
- Extra credit: rastgele tile (çim/taş) yerleşimi ile zemin.
- Scratch Card plugin'i (`Assets/ScratchCard/`) kullanılacak — ama klasik "kazı-kazan" UI'ı değil, **karakter ve etrafındaki kılıçların hareket ederken zemini "kazıyıp" altındaki dokuyu açığa çıkarması** (referans görsellerdeki koyu toprak → açık kum geçişi).
- Teslim: `YourName_LoopGames.zip` (Library/Temp hariç proje dosyaları).

---

## 2. Brainstorming'de netleşen kararlar (kullanıcı onaylı)

Referans ekran görüntüleri incelendi (io-tarzı meyve karakterler, bayrak+kullanıcı adı, can barı,
karakter etrafında dönen çoklu kılıç, koyu "fog/toprak" katmanının kazınması, direkli çit sınırı).

| Konu | Karar |
|---|---|
| Perspektif | Top-down, **sabit zoom** (dinamik zoom YOK — ekran görüntülerindeki farklar sadece farklı çözünürlüklerden kaynaklıydı) |
| Kontrol | Mobil + Editor mouse — **görünmez sürükle-hareket** (joystick grafiği yok), new Input System |
| Saldırı | **Ayrı buton YOK.** Kılıçlar karakter etrafında sürekli döner, bir düşmana/düşman kılıcına değince otomatik hasar/çarpışma olur. Oyuncu sadece hareket eder. |
| Kılıç taşıma | **Sınırsız** (kod tarafında hard cap yok) — karakter topladıkça etrafında kılıç sayısı artar |
| Kılıç çarpışması | İki kılıç çarpışınca ikisi de yok olur (mevcut case kuralı) |
| Kılıçsız can | 3 vuruşta ölür |
| Hit feedback | Vuruş anında ~0.5sn **beyaz flash**, sonra normale döner (ölüm değil, sadece görsel) |
| Ölüm efekti | Ayrı efekt: karakter **scale'i sıfıra küçülerek** kayboluyor |
| Karakter ayrımı | Sprite **tint rengi** + **bayrak rozeti** (tr/us/cn/jp) birlikte kullanılacak |
| Harita | Kameradan büyük (ekrana sığmayan), **takip kamerası** gerekiyor |
| Zemin | 4 verilen doku (`tile-grass-2`, `tile-grass-9`, `tile-ground`, `tile-stone-7`) hücre başına **uniform rastgele** seçilir (şimdilik — scratch/fog katmanı ayrı, sonra eklenecek) |
| Çit | `map-decoration-fence-corner.png` aslında **direk (post)**, `horizontal`/`vertical` ise direkler arasını bağlayan **ray/tahta**. Direkler her ~3 birimde bir tekrarlanır, aralar raylarla dolar (bkz. §5.2). |
| Karakter animasyonu | **Kod ile (procedural)**, Animator/clip YOK — bacaklar kalça pivotundan sallanır, gövde Y-scale squash/stretch yapar, hıza bağlı |
| ScratchCard kullanımı | Menü/ödül ekranı DEĞİL — gameplay VFX: karakter + kılıç pozisyonları zemin dokusunu runtime'da "kazır" |

---

## 3. Mimari prensipler (SOLID nasıl uygulandı)

- **Interface-first**: Her sistem somut sınıflar yerine `Assets/Scripts/Core/Interfaces/` altındaki
  arayüzlere bağımlı (`IHealth`, `IDamageable`, `IMovable`, `IMoveInputProvider`, `IWeaponCarrier`,
  `ISword`, `ICharacterIdentity`). Örn: `CharacterMovement2D` hem oyuncunun (`DragInputProvider`)
  hem AI'nin aynı `IMoveInputProvider` arayüzü üzerinden hareket girdisi vermesini sağlıyor —
  AI eklerken `CharacterMovement2D`'ye hiç dokunmayacaksın, sadece yeni bir `IMoveInputProvider`
  implementasyonu (örn. `AIMoveInputProvider`) yazacaksın.
- **Event-driven decoupling**: `Assets/Scripts/Core/Events/GameEventBus.cs` generic, tip-güvenli
  bir pub/sub sistemi. Sistemler birbirine referans vermeden haberleşir (`GameEvents.cs`'deki
  struct event'ler: `CharacterSpawnedEvent`, `CharacterDiedEvent`, `SwordPickedUpEvent`,
  `SwordClashEvent`, `MatchEndedEvent`). Yeni bir sistem eklerken (örn. UI can barı) doğrudan
  health component'e referans vermek yerine `GameEventBus.Subscribe<CharacterDiedEvent>(...)` kullan.
- **ScriptableObject config'ler** (`Assets/Scripts/Core/Config/`): `CharacterConfig`, `SwordConfig`,
  `GameConfig`. Tasarım değerleri (can, hız, hasar, spawn aralığı) kod içine gömülü değil, Inspector'dan
  ayarlanabilir asset'ler (`Assets/Configs/*.asset`). Yeni bir tunable değer eklerken buraya ekle.
  Not: `CharacterConfig`/`SwordConfig` asset'leri **oluşturuldu ama henüz hiçbir component'e
  bağlanmadı** (Combat/AI modülleri onları tüketecek).
- **Single Responsibility**: Harita üretimi bile 5 küçük sınıfa bölündü
  (`IMapGenerator` → `MapGenerator` orkestratör, `GroundLayerBuilder`, `FenceBorderBuilder`,
  `ArenaBoundaryBuilder`, `TileFactory` paylaşılan yardımcı). Yeni bir üretim algoritması
  (örn. Perlin noise ile ada şekli) eklemek istersen sadece yeni bir `IMapGenerator`
  implementasyonu yazman yeterli, `GameManager` ya da başka hiçbir sistem etkilenmez (Open/Closed).
- **Namespace yapısı**: `AppleGrapple.Core`, `AppleGrapple.Core.Config`, `AppleGrapple.Core.Events`,
  `AppleGrapple.Map`, `AppleGrapple.Player`, `AppleGrapple.Movement`, `AppleGrapple.Animation`,
  `AppleGrapple.CameraControl`. Yeni modüller için aynı desende namespace aç (örn.
  `AppleGrapple.Combat`, `AppleGrapple.AI`, `AppleGrapple.Identity`, `AppleGrapple.VFX`, `AppleGrapple.UI`).

---

## 4. Şu ana kadar tamamlanan modüller

### Modül 1 — Core Architecture ✅
`Assets/Scripts/Core/`
- `Interfaces/IHealth.cs`, `IDamageable.cs`, `IMovable.cs`, `IMoveInputProvider.cs`,
  `ICharacterIdentity.cs`, `ISword.cs`, `IWeaponCarrier.cs`
- `Events/GameEventBus.cs` (generic pub/sub), `Events/GameEvents.cs` (event struct'ları)
- `Config/CharacterConfig.cs`, `Config/SwordConfig.cs`, `Config/GameConfig.cs` (ScriptableObject)
- `GameManager.cs` — sadece maç kazananını takip eder (`CharacterSpawnedEvent`/`CharacterDiedEvent`
  dinler, tek kişi kalınca `MatchEndedEvent` yayınlar). Kılıç/AI/hareket mantığına karışmaz.

Asset'ler: `Assets/Configs/GameConfig.asset` (TotalCharacterCount=4, ArenaHalfExtents=(20,20) →
40x40 tile grid), `CharacterConfig.asset`, `SwordConfig.asset` (ikisi de default değerlerde,
henüz kimse tüketmiyor).

Sahne: `GameManager` GameObject'i, `GameManager` component'i + `gameConfig` bağlı.

### Modül 2 — Harita üretimi ✅
`Assets/Scripts/Map/`
- `IMapGenerator.cs` — arayüz (`ArenaHalfExtents`, `Generate()`)
- `TileFactory.cs` — sprite boyutundan bağımsız, istenen dünya boyutuna göre tile GameObject'i
  üreten paylaşılan yardımcı (`CreateTile`, `CreateStretchedTile` — genişlik/yükseklik ayrı ayrı
  ölçeklenebilir, çit rayları için kullanılıyor)
- `GroundLayerBuilder.cs` — 4 zemin dokusundan (`groundSprites[]`) hücre başına uniform rastgele
  seçim yaparak grid dolduruyor
- `FenceBorderBuilder.cs` — **FİNAL (kullanıcı onaylı) desen**: her gerçek köşede (4 adet) ve
  her kenar boyunca `postSpacing` (varsayılan 3) tile'da bir `post` (hiç rotate edilmeyen
  "corner" sprite'ı) yerleştiriliyor; iki post arasındaki her tek tile ise sırasıyla
  `horizontalConnector`/`verticalConnector` sprite'ı ile dolduruluyor. Desen tam olarak:
  `Corner, SpriteX, SpriteX, SpriteX, Corner, ...`. Görsel olarak MCP screenshot ile
  doğrulandı ve kullanıcı tarafından onaylandı — başka değişiklik gerekmiyor.
- `ArenaBoundaryBuilder.cs` — zemin grid'inin hemen dışına **görünmez 4 adet BoxCollider2D duvar**
  koyar (fence sadece görsel, fiziksel sınırı bu sağlıyor).
- `MapGenerator.cs` — orkestratör: `Ground`, `FenceBorder`, `ArenaBoundary` alt objelerini
  `Start()` içinde üretir. Inspector'da `gameConfig`, `groundSprites[4]`, `fenceHorizontal/Vertical/Corner`
  bağlı.

Sahne: `MapGenerator` GameObject'i, tüm referanslar bağlı. `Main Camera` Orthographic
(`orthographicSize=6`).

### Modül 3 — Player Controller ✅ (kısmen test edildi)
- `Core/Interfaces/IMoveInputProvider.cs` — `Vector2 GetMoveInput()`
- `Player/DragInputProvider.cs` — **new Input System** (`Mouse.current`, `Touchscreen.current`)
  ile basılı-tut-sürükle input. Görünmez joystick: basılan ilk nokta merkez kabul edilir, oradan
  sürüklenen mesafe (max `maxDragDistancePixels`) yön+büyüklük (0..1) üretir.
  ⚠️ **ÖNEMLİ**: Bu projede Active Input Handling **"Input System Package (New)"** olarak ayarlı
  (Project Settings → Player → Active Input Handling). Legacy `UnityEngine.Input` kullanmaya
  çalışırsan `InvalidOperationException` alırsın — her zaman `UnityEngine.InputSystem`
  (`Mouse.current`, `Touchscreen.current`, `Keyboard.current` vb.) kullan.
- `Movement/CharacterMovement2D.cs` — `Rigidbody2D` tabanlı, **jenerik** hareket uygulayıcı
  (`IMovable` implement eder). `moveInputProviderSource` (herhangi bir `IMoveInputProvider`) ve
  `characterConfig`'den hız alır. AI karakterleri de bu sınıfı kullanacak, sadece farklı bir
  `IMoveInputProvider` vereceksin (bkz. §5.4).
  Not: Unity 6'da `Rigidbody2D.velocity` yerine `linearVelocity` kullanılıyor (kod bunu zaten yapıyor).
- `Animation/CharacterWalkAnimator.cs` — **kod ile** prosedürel yürüyüş: `leftLegPivot`/`rightLegPivot`
  (kalça noktaları) `leftLegRestAngle=15`/`rightLegRestAngle=-15` durağan açılardan başlayıp,
  hızla orantılı sinüs dalgasıyla ters fazda sallanıyor; gövde Y-scale'i hafif squash/stretch yapıyor.
  `CharacterMovement2D.CurrentVelocity.magnitude`'i okuyor.
- `CameraControl/CameraFollow.cs` — basit `SmoothDamp` takip, **sınır clamp'i yok** (kullanıcı
  isteğiyle kaldırıldı — karakter her zaman ekran merkezinde, fiziksel sınırı `ArenaBoundaryBuilder`
  zaten sağlıyor).

Sahne: `Player` GameObject'i (`Rigidbody2D` gravityScale=0, `CircleCollider2D`,
`DragInputProvider`, `CharacterMovement2D`, `CharacterWalkAnimator`) + `Body`/`LeftLeg`/`RightLeg`
alt hiyerarşisi (bacaklar kalça pivotundan sarkıyor — kullanıcı bunu editörde elle kurdu).
`Main Camera`'ya `CameraFollow` eklendi, `target=Player`.

**Kullanıcı tarafından PLAY MODE'DA MANUEL TEST edilmesi gerekenler:**
- [ ] Sürükle-hareket gerçekten çalışıyor mu (mouse ile Editor'de, sonra cihazda touch ile)
- [ ] Bacak animasyonu yürürken doğru görünüyor mu (V şeklinden sallanmaya geçiş)
- [ ] Karakter çitin dışına çıkabiliyor mu (çıkmamalı — `ArenaBoundaryBuilder` çalışıyor mu kontrol et)
- [ ] Çit görseli (rail/post) referans görsele yeterince yakın mı

---

## 5. Geri kalan modüller — nasıl yapılacak (adım adım)

Aşağıdaki sıra önerilir (her biri bir öncekinin üzerine oturur). Her modülde **önce arayüzü
yaz, sonra somut implementasyonu**, mevcut `GameEventBus`/config sistemini kullan.

### 5.1 Modül 4 — Combat System (kılıç toplama, orbit, çarpışma, hasar, ölüm)

Bu proje için en kritik modül. Önerilen alt parçalar:

1. **`Sword` MonoBehaviour** (`Assets/Scripts/Combat/Sword.cs`) — `ISword` implement eder.
   - `Damage` (SwordConfig'den), `Owner` (null = yerde duruyor, dolu = birinin orbit'inde).
   - Durum makinesi: `OnGround` → `Orbiting` → (çarpışma ile) `Destroyed`.
   - `OnGround` iken bir `CircleCollider2D` (trigger) ile karakterin `IWeaponCarrier`'ına
     temas edince `AddSword` çağrılır, `SwordPickedUpEvent` yayınlanır (`GameEventBus.Publish`).
   - `Orbiting` iken kendi trigger collider'ı: başka bir karakterin `IDamageable`'ına değerse
     hasar ver + kendi kılıcını yok et (tek taraflı hasar, kılıç harcanmaz mı yoksa harcanır mı
     — **case dokümanında yazmıyor, kendi tercihine göre karar ver**, örneğin çarpışma sadece
     iki KILIÇ birbirine değince ikisi de yok olsun, bir kılıç bir KARAKTERE değince hasar verip
     kılıç kalıcı olsun — böylece "sürekli dönen kılıçlar" mantığı korunur).
   - Kılıç-kılıç çarpışması: iki farklı sahibin orbit kılıçları çakışırsa ikisi de `Destroy()`
     çağrılır + `SwordClashEvent` yayınlanır.

2. **`WeaponOrbit` MonoBehaviour** (`Assets/Scripts/Combat/WeaponOrbit.cs`) — `IWeaponCarrier`
   implement eder. Karakterin üstüne eklenir (Player ve AI ortak).
   - `List<ISword> carriedSwords`.
   - `Update()`'te her kılıcı, sahibinin etrafında eşit açılarla dağıtıp döndürür
     (`CharacterConfig.OrbitRadius`, `OrbitRotationSpeed` zaten hazır, kullan). N kılıç varsa
     her biri `360/N` derece arayla yerleşir, `Time.time * OrbitRotationSpeed` ile döner.
   - `AddSword`/`RemoveSword` çağrıldığında açı dağılımını yeniden hesapla.
   - `IsArmed => carriedSwords.Count > 0`.

3. **`HealthComponent` MonoBehaviour** (`Assets/Scripts/Combat/HealthComponent.cs`) —
   `IHealth` + `IDamageable` implement eder.
   - `CharacterConfig.UnarmedHitsToDie` kadar can (örn. 3 "can birimi" = `MaxHealth=3`).
   - `TakeDamage(amount, instigator)`: **sadece `WeaponOrbit.IsArmed == false` iken** can azaltılsın
     (case kuralı: "kılıcı olmayan karakter 3 vuruşta ölür" → silahlı karakter muhtemelen
     hasar almıyor, sadece silahsızken savunmasız; bunu netleştirmek istersen kendi yorumunu
     uygula, örn. silahlıyken de hasar alsın ama daha dayanıklı olsun).
   - Hasar alınca `HitFlash` tetiklenir (aşağıda), can 0 olunca `Died` event'i + ölüm efekti.

4. **`HitFlashEffect` MonoBehaviour** (`Assets/Scripts/Combat/HitFlashEffect.cs`) —
   `HealthComponent.HealthChanged`'i dinler (ya da doğrudan `IDamageable.TakeDamage` çağrısından
   tetiklenir), `Body`/`LeftLeg`/`RightLeg` `SpriteRenderer.color`'ını `CharacterConfig.HitFlashColor`'a
   çevirip `HitFlashDuration` sonra normale döndürür (bir coroutine ya da `Update`'te sayaç ile).

5. **`DeathEffect` MonoBehaviour** (`Assets/Scripts/Combat/DeathEffect.cs`) —
   `HealthComponent.Died` event'ini dinler, `transform.localScale`'i `DeathShrinkDuration`
   boyunca sıfıra indirir (Lerp/coroutine), sonra `GameEventBus.Publish(new CharacterDiedEvent(...))`
   + `Destroy(gameObject)`.

6. **`SwordSpawner` MonoBehaviour** (`Assets/Scripts/Combat/SwordSpawner.cs`) —
   `SwordConfig.MinSpawnInterval`/`MaxSpawnInterval` arası rastgele bekleyip `MaxSwordsOnGround`
   sınırını aşmadan arena içinde rastgele bir konumda (`GameConfig.ArenaHalfExtents` içinde,
   `Random.Range`) yeni bir `Sword` prefab'ı instantiate eder. Spawn öncesi kısa bir
   "bubble" görsel efekti istersen (`apple-gun-gameplay-pickup-item-weapon-bubble-2.png`)
   ayrı bir `SwordSpawnBubbleEffect` component'iyle ekleyebilirsin (opsiyonel polish).

**Prefab önerisi**: `Assets/Prefabs/Sword.prefab` oluştur (SpriteRenderer +
`apple-gun-weapon-red-sword.png` + `CircleCollider2D` trigger + `Sword.cs`). `SwordSpawner`
bu prefab'ı instantiate etsin.

**Test sırası**: Önce tek oyuncu + yerde birkaç kılıç + toplama çalışsın, sonra orbit görseli,
sonra iki karakter (elle sahneye ikinci bir "TestEnemy" koy, henüz AI yazmadan) ile çarpışma/hasar
test et, en son `SwordSpawner`'ı bağla.

### 5.2 Modül 5 — Enemy AI

`Assets/Scripts/AI/` altında:

1. **`AIMoveInputProvider` MonoBehaviour** (`IMoveInputProvider` implement eder) —
   `CharacterMovement2D`'nin `moveInputProviderSource` alanına bunu bağlayacaksın (Player'da
   `DragInputProvider` neyse, AI'da bu). Böylece Modül 3'teki hareket kodu **hiç değişmeden** AI'da
   da çalışır.
2. **Basit state machine** (`Assets/Scripts/AI/AIController.cs` + enum `AIState { SeekSword, SeekEnemy, Attack, Flee }`
   ya da her state için ayrı küçük strateji sınıfı — Strategy pattern tercih edilirse
   `IAIState` arayüzü + `SeekSwordState`, `SeekEnemyState` vb. sınıflar, `AIController` sadece
   aktif state'i çalıştırıp geçişleri yönetir).
   - **SeekSword**: `IWeaponCarrier.IsArmed == false` ise sahnedeki en yakın `Sword` (OnGround
     durumda) objesine yönel (`AIMoveInputProvider.GetMoveInput()` o yöne bakan vektörü döndürsün).
   - **SeekEnemy**: silahlıysa, `GameManager.AliveCharacters` (ya da `FindObjectsOfType`) içinden
     en yakın rakibe yönel.
   - **Attack/Kaçınma**: zaten orbit kılıçlar otomatik hasar verdiği için ayrı bir "saldırı" state'i
     gerekmeyebilir — sadece rakibe yeterince yaklaşıp etrafında dönmek yeterli. "Tehlikeden
     kaçınma" için basitçe: silahsızken ve yakınında silahlı bir düşman varsa ondan uzaklaşan
     yöne hareket et (Flee state).
   - Karar mantığını `FixedUpdate`'te değil, performans için `~0.2sn`'de bir (bir `Coroutine`
     ya da sayaç ile) yeniden değerlendir; sadece hareket yönü her frame güncellensin.
3. Sahneye 3 adet AI karakteri: Player prefab'ının aynısını kopyala (`Prefab` yap), `DragInputProvider`
   yerine `AIMoveInputProvider` koy, `AIController` ekle.

### 5.3 Modül 6 — Karakter Kimliği (tint + bayrak)

`Assets/Scripts/Identity/` altında:

1. **`CharacterIdentity` MonoBehaviour** (`ICharacterIdentity` implement eder) — `displayName`,
   `tintColor`, `flagIcon` alanları (Inspector'dan ya da spawn sırasında kod ile atanır).
2. Spawn sırasında (bir `MatchSetup`/`CharacterSpawner` script'i ile, Player + 3 AI'yi
   `GameConfig.TotalCharacterCount`'a göre üretirsin) her karaktere sırayla farklı bir tint rengi
   (`Body`/`LeftLeg`/`RightLeg` SpriteRenderer'larının `color`'ını çarp) ve bir bayrak
   (`Assets/UI/textures/flags/{tr,us,cn,jp}.png`) ata. Bayrağı, karakterin üstünde duran küçük bir
   `SpriteRenderer` (child GameObject, `sortingOrder` yüksek) ile göster.
3. `Awake()`/`Start()`'ta `GameEventBus.Publish(new CharacterSpawnedEvent(this))` çağırmayı unutma
   — `GameManager` bunu dinliyor.

### 5.4 Modül 7 — Ground-Scratch VFX (ScratchCard entegrasyonu)

En riskli/araştırma gerektiren modül. Önce şunu yap:
1. `Assets/ScratchCard/Example.unity` sahnesini aç, `ScratchCard.cs` ve `ScratchCardManager.cs`'i
   oku — muhtemelen bir `RenderTexture`/mask üzerine mouse pozisyonuyla "silme" (erase) yapan bir
   API sunuyor (`Erase(Vector2 position, float radius)` gibi bir public metod arıyor ol).
2. Senin ihtiyacın: mouse yerine **karakterin dünya pozisyonunu ekran/UV koordinatına çevirip**
   her frame (ya da her birkaç frame'de) o noktada (ve her orbit kılıcının pozisyonunda) erase
   çağırmak. Yeni bir `Assets/Scripts/VFX/GroundScratchController.cs` yaz:
   - Sahnede zemini kaplayan bir `ScratchCard` instance'ı olsun (tüm arena boyutunda, koyu
     "toprak" dokusu üstte, açık "kum" dokusu altta — iki katman).
   - Her karakter (`GameEventBus.Subscribe<CharacterSpawnedEvent>`) spawn olduğunda bu controller'a
     kaydolsun; `Update`'te her karakterin + her orbit kılıcının pozisyonunu `ScratchCard`'ın
     erase metoduna verilecek koordinata çevirip çağır.
3. Bu modül görsel olarak karmaşık olabilir — zaman kalmazsa **statik/tamamlanmış bir zemin**
   (Modül 2'deki gibi) ile devam edip bu VFX'i "nice-to-have" olarak bırakmak makul bir tercih
   (case dokümanı zaten "extra credit" diyor, zorunlu değil).

### 5.5 Modül 8 — UI/HUD

`Assets/Scripts/UI/` altında (Unity UI/Canvas ile):
1. Her karakterin üstünde bir can barı (World Space Canvas, `HealthComponent.HealthChanged`
   event'ini dinleyip `Image.fillAmount` günceller) — zaten referans görsellerde var (yeşil/turuncu/kırmızı bar).
2. Maç sonu ekranı: `GameEventBus.Subscribe<MatchEndedEvent>` dinleyip "Kazandın"/"Kaybettin"
   panelini göster, yeniden başlat butonu (`SceneManager.LoadScene` ile mevcut sahneyi reload).

### 5.6 Modül 9 — Polish

- Ses efektleri (kılıç çarpışma, ölüm, adım sesleri) — `AudioSource` + basit bir `AudioManager`.
- Ekstra "juice": hit-stop (çok kısa `Time.timeScale` düşüşü), ekran sarsıntısı (kamera
  offset'i birkaç frame rastgele oynatma).
- Performans/optimizasyon: `Object Pooling` (kılıçlar sürekli spawn/destroy oluyor, `Destroy`
  yerine pool kullanmayı düşün — küçük prototipte şart değil ama "Optimizasyon" değerlendirme
  kriteri var).
- Build alıp gerçekten hatasız çalıştığını doğrula (`File > Build Settings`).

---

## 6. Bilinen açık maddeler / dikkat edilmesi gerekenler

- **Player drag-movement** ve **yürüme animasyonu** MCP üzerinden simüle edilemediği için
  gerçek cihaz/Editor'de manuel test edilmedi — önce bunu doğrula, sonra Combat'a geç.
- **Active Input Handling = "Input System Package (New)"** — legacy `Input` sınıfını ASLA kullanma,
  her yerde `UnityEngine.InputSystem` (`Mouse.current`, `Touchscreen.current`) kullan.
- **Unity 6 Rigidbody2D**: `velocity` yerine `linearVelocity` kullanılıyor, tutarlı ol.
- **CharacterConfig/SwordConfig** asset'leri (`Assets/Configs/`) oluşturuldu ama henüz hiçbir
  component'e bağlı değil — Combat modülünü yazarken Inspector'dan bağlamayı unutma.
- Kılıç-karakter hasar kuralı (silahlıyken hasar alınır mı, sadece silahsızken mi) case
  dokümanında net değil — §5.1'de not edildiği gibi kendi yorumunu uygulayabilirsin, sadece
  kararını burada/README'de belgelemen değerlendirme açısından iyi olur.
- ScratchCard entegrasyonu zaman alırsa önceliği düşür, temel oynanışı (Combat+AI+Win condition)
  önce bitir.

---

## 7. Dosya haritası (şu ana kadar)

```
Assets/Scripts/
  Core/
    Interfaces/  IHealth.cs, IDamageable.cs, IMovable.cs, IMoveInputProvider.cs,
                 ICharacterIdentity.cs, ISword.cs, IWeaponCarrier.cs
    Events/      GameEventBus.cs, GameEvents.cs
    Config/      CharacterConfig.cs, SwordConfig.cs, GameConfig.cs
    GameManager.cs
  Map/
    IMapGenerator.cs, TileFactory.cs, GroundLayerBuilder.cs, FenceBorderBuilder.cs,
    ArenaBoundaryBuilder.cs, MapGenerator.cs
  Player/
    DragInputProvider.cs
  Movement/
    CharacterMovement2D.cs
  Animation/
    CharacterWalkAnimator.cs
  CameraControl/
    CameraFollow.cs

Assets/Configs/
  GameConfig.asset, CharacterConfig.asset, SwordConfig.asset

Assets/Scenes/MainScene.unity  (GameManager, MapGenerator, Player, Main Camera, Directional Light)
```

**Henüz yok (senin yazacakların)**:
```
Assets/Scripts/Combat/    Sword.cs, WeaponOrbit.cs, HealthComponent.cs, HitFlashEffect.cs,
                          DeathEffect.cs, SwordSpawner.cs
Assets/Scripts/AI/        AIMoveInputProvider.cs, AIController.cs (+ state sınıfları)
Assets/Scripts/Identity/  CharacterIdentity.cs, CharacterSpawner.cs
Assets/Scripts/VFX/       GroundScratchController.cs
Assets/Scripts/UI/        HealthBarUI.cs, MatchEndUI.cs
Assets/Prefabs/           Sword.prefab, Player.prefab (mevcut Player'dan), Enemy.prefab
```

---

## 8. Önerilen çalışma sırası (özet)

1. Modül 3'ü manuel test et (drag-move, animasyon, sınır collider).
2. Modül 4 (Combat) — önce Sword + WeaponOrbit + toplama, sonra Health/HitFlash/Death, sonra Spawner.
3. Modül 5 (AI) — `AIMoveInputProvider` + basit state machine, 3 AI karakteri sahneye ekle.
4. Modül 6 (Identity) — tint + bayrak, `CharacterSpawner` ile 4 karakteri kodla üret.
5. Win condition'ı `GameManager` zaten yönetiyor — Modül 8 (UI) ile görünür kıl.
6. Zaman kalırsa Modül 7 (ScratchCard VFX) ve Modül 9 (Polish).

Kolay gelsin — mimari zaten SOLID prensiplere göre kuruldu, yeni sistemleri mevcut
interface/event-bus/config altyapısına oturtarak ilerlersen kod tabanı tutarlı kalır.
