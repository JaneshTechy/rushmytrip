using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class RSFLY : DbContext
    {
        public RSFLY()
        {
        }

        public RSFLY(DbContextOptions<RSFLY> options)
            : base(options)
        {
        }

        public virtual DbSet<AddOnService> AddOnServices { get; set; }
        public virtual DbSet<AddOnServiceMaster> AddOnServiceMasters { get; set; }
        public virtual DbSet<AdminPage> AdminPages { get; set; }
        public virtual DbSet<AdminUser> AdminUsers { get; set; }
        public virtual DbSet<AdminUserRole> AdminUserRoles { get; set; }
        public virtual DbSet<AirlineCodeShare> AirlineCodeShares { get; set; }
        public virtual DbSet<AirlineDetail> AirlineDetails { get; set; }
        public virtual DbSet<AirlineRoute> AirlineRoutes { get; set; }
        public virtual DbSet<AirlineTable> AirlineTables { get; set; }
        public virtual DbSet<AirlineTable1> AirlineTables1 { get; set; }
        public virtual DbSet<Apicredential> Apicredentials { get; set; }
        public virtual DbSet<ApplyCoupon> ApplyCoupons { get; set; }
        public virtual DbSet<BannerDetail> BannerDetails { get; set; }
        public virtual DbSet<BlockAirlineRule> BlockAirlineRules { get; set; }
        public virtual DbSet<BlockBaggageMaster> BlockBaggageMasters { get; set; }
        public virtual DbSet<BlockedAirline> BlockedAirlines { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<BlogPost> BlogPosts { get; set; }
        public virtual DbSet<BlogReview> BlogReviews { get; set; }
        public virtual DbSet<BookQuoteMaster> BookQuoteMasters { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingConversation> BookingConversations { get; set; }
        public virtual DbSet<BookingFlghtDetail> BookingFlghtDetails { get; set; }
        public virtual DbSet<BookingPaxDetail> BookingPaxDetails { get; set; }
        public virtual DbSet<BookingsB2c> BookingsB2cs { get; set; }
        public virtual DbSet<BookingsCardDetail> BookingsCardDetails { get; set; }
        public virtual DbSet<CacheGd> CacheGds { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CityGroup> CityGroups { get; set; }
        public virtual DbSet<CityMaster> CityMasters { get; set; }
        public virtual DbSet<Continent> Continents { get; set; }
        public virtual DbSet<CopyOffersWebsite> CopyOffersWebsites { get; set; }
        public virtual DbSet<CouponMaster> CouponMasters { get; set; }
        public virtual DbSet<CurrencyMaster> CurrencyMasters { get; set; }
        public virtual DbSet<DeepLinkTracker> DeepLinkTrackers { get; set; }
        public virtual DbSet<DestinationAirportMapping> DestinationAirportMappings { get; set; }
        public virtual DbSet<DestinationFareType> DestinationFareTypes { get; set; }
        public virtual DbSet<DynamicPage> DynamicPages { get; set; }
        public virtual DbSet<DynamicQueryForm> DynamicQueryForms { get; set; }
        public virtual DbSet<DynamicQueryForm1> DynamicQueryForms1 { get; set; }
        public virtual DbSet<EquipmentAir> EquipmentAirs { get; set; }
        public virtual DbSet<ExcelUploadStatusLog> ExcelUploadStatusLogs { get; set; }
        public virtual DbSet<ExternalClicksMetaService> ExternalClicksMetaServices { get; set; }
        public virtual DbSet<ExtraService> ExtraServices { get; set; }
        public virtual DbSet<Faq> Faqs { get; set; }
        public virtual DbSet<FaqMaster> FaqMasters { get; set; }
        public virtual DbSet<FareInfoNego> FareInfoNegos { get; set; }
        public virtual DbSet<Fareinfo> Fareinfos { get; set; }
        public virtual DbSet<FareinfoMaster> FareinfoMasters { get; set; }
        public virtual DbSet<FeatureFlight> FeatureFlights { get; set; }
        public virtual DbSet<FeaturedAirline> FeaturedAirlines { get; set; }
        public virtual DbSet<FlightBooking> FlightBookings { get; set; }
        public virtual DbSet<FlightSearchCache> FlightSearchCaches { get; set; }
        public virtual DbSet<GoogleOauthToken> GoogleOauthTokens { get; set; }
        public virtual DbSet<HolidayDetail> HolidayDetails { get; set; }
        public virtual DbSet<HomeOffersCoupon> HomeOffersCoupons { get; set; }
        public virtual DbSet<HomePackageCoupon> HomePackageCoupons { get; set; }
        public virtual DbSet<HomeSequence> HomeSequences { get; set; }
        public virtual DbSet<HotelMarkupDetail> HotelMarkupDetails { get; set; }
        public virtual DbSet<HotelMasterSelected> HotelMasterSelecteds { get; set; }
        public virtual DbSet<InOutAirlineMaster> InOutAirlineMasters { get; set; }
        public virtual DbSet<InclusionExclusion> InclusionExclusions { get; set; }
        public virtual DbSet<InsuranceOffline> InsuranceOfflines { get; set; }
        public virtual DbSet<LikedFareBasis> LikedFareBases { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<ManageFooter> ManageFooters { get; set; }
        public virtual DbSet<ManageHeader> ManageHeaders { get; set; }
        public virtual DbSet<ManageMenu> ManageMenus { get; set; }
        public virtual DbSet<ManageUsp> ManageUsps { get; set; }
        public virtual DbSet<MarkupDetail> MarkupDetails { get; set; }
        public virtual DbSet<MarkupDetailsAdvance> MarkupDetailsAdvances { get; set; }
        public virtual DbSet<MarkupDetailsHotel> MarkupDetailsHotels { get; set; }
        public virtual DbSet<MarkupGroup> MarkupGroups { get; set; }
        public virtual DbSet<MarkupGroupMaster> MarkupGroupMasters { get; set; }
        public virtual DbSet<MarkupMaster> MarkupMasters { get; set; }
        public virtual DbSet<MarkupRule> MarkupRules { get; set; }
        public virtual DbSet<MasterCondition> MasterConditions { get; set; }
        public virtual DbSet<MasterOffersCategory> MasterOffersCategories { get; set; }
        public virtual DbSet<MetaSearchDestination> MetaSearchDestinations { get; set; }
        public virtual DbSet<MissingCity> MissingCities { get; set; }
        public virtual DbSet<NewsletterEmail> NewsletterEmails { get; set; }
        public virtual DbSet<OfferMarkupTblUpdateDetail> OfferMarkupTblUpdateDetails { get; set; }
        public virtual DbSet<OffersCategory> OffersCategories { get; set; }
        public virtual DbSet<OffersSpecial> OffersSpecials { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentEvent> PaymentEvents { get; set; }
        public virtual DbSet<PaymentLink> PaymentLinks { get; set; }
        public virtual DbSet<PaymentsBlock> PaymentsBlocks { get; set; }
        public virtual DbSet<PermittedAirline> PermittedAirlines { get; set; }
        public virtual DbSet<PermittedCountry> PermittedCountries { get; set; }
        public virtual DbSet<PopularDestination> PopularDestinations { get; set; }
        public virtual DbSet<PopularDestinationFlight> PopularDestinationFlights { get; set; }
        public virtual DbSet<Quotation> Quotations { get; set; }
        public virtual DbSet<Quotation1> Quotations1 { get; set; }
        public virtual DbSet<QuoteExtraService> QuoteExtraServices { get; set; }
        public virtual DbSet<QuoteHotelDetail> QuoteHotelDetails { get; set; }
        public virtual DbSet<QuoteTripDetail> QuoteTripDetails { get; set; }
        public virtual DbSet<RecentSearch> RecentSearches { get; set; }
        public virtual DbSet<SeoEntity> SeoEntities { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<SiteAddOn> SiteAddOns { get; set; }
        public virtual DbSet<SiteAddonmaster> SiteAddonmasters { get; set; }
        public virtual DbSet<SiteId> SiteIds { get; set; }
        public virtual DbSet<SiteMaster> SiteMasters { get; set; }
        public virtual DbSet<SiteMiscService> SiteMiscServices { get; set; }
        public virtual DbSet<SiteSeo> SiteSeos { get; set; }
        public virtual DbSet<SiteServiceMaster> SiteServiceMasters { get; set; }
        public virtual DbSet<SpecialJourney> SpecialJourneys { get; set; }
        public virtual DbSet<SpecialOffer> SpecialOffers { get; set; }
        public virtual DbSet<SpecialOfferCategory> SpecialOfferCategories { get; set; }
        public virtual DbSet<SpecialOfferItnaryDetail> SpecialOfferItnaryDetails { get; set; }
        public virtual DbSet<SpecialOfferMiscellaneou> SpecialOfferMiscellaneous { get; set; }
        public virtual DbSet<StCity> StCities { get; set; }
        public virtual DbSet<StCityName> StCityNames { get; set; }
        public virtual DbSet<StCountry> StCountries { get; set; }
        public virtual DbSet<StCountryName> StCountryNames { get; set; }
        public virtual DbSet<StartRatingSelected> StartRatingSelecteds { get; set; }
        public virtual DbSet<Taxinfo> Taxinfos { get; set; }
        public virtual DbSet<TblCampaign> TblCampaigns { get; set; }
        public virtual DbSet<TblHorizontalBanner> TblHorizontalBanners { get; set; }
        public virtual DbSet<TblmanageIp> TblmanageIps { get; set; }
        public virtual DbSet<TempSiteMasterzz> TempSiteMasterzzs { get; set; }
        public virtual DbSet<TemplateMaster> TemplateMasters { get; set; }
        public virtual DbSet<Testimonial> Testimonials { get; set; }
        public virtual DbSet<TheameColor> TheameColors { get; set; }
        public virtual DbSet<UmrahOfferDetail> UmrahOfferDetails { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserSearch> UserSearches { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<Userlog> Userlogs { get; set; }
        public virtual DbSet<Website> Websites { get; set; }
        public virtual DbSet<WebsiteContact> WebsiteContacts { get; set; }
        public virtual DbSet<WlsCustomBusinessOffer> WlsCustomBusinessOffers { get; set; }
        public virtual DbSet<WlsCustomFooter> WlsCustomFooters { get; set; }
        public virtual DbSet<WlsCustomHeader> WlsCustomHeaders { get; set; }
        public virtual DbSet<Zztemp> Zztemps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=195.250.31.139;Database=universal_whiteLabelCrm-EC;User Id=sa;Password=2R0dsrPhVa5EDCP$;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AddOnService>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AddOnService");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("endDate");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Service)
                    .HasMaxLength(50)
                    .HasColumnName("service");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("startDate");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<AddOnServiceMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AddOnService_Master");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.Code)
                    .HasMaxLength(3)
                    .HasColumnName("code")
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PaymentUrl)
                    .HasMaxLength(50)
                    .HasColumnName("paymentUrl");

                entity.Property(e => e.Service)
                    .HasMaxLength(50)
                    .HasColumnName("service");
            });

            modelBuilder.Entity<AdminPage>(entity =>
            {
                entity.HasKey(e => e.PageId)
                    .HasName("PK__AdminPag__C565B10472441BD8");

                entity.Property(e => e.Categories)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Pages)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__AdminUse__1788CC4CC7C66286");

                entity.HasIndex(e => e.UserEmail, "UQ__AdminUse__08638DF8BAE083DD")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "UQ__AdminUse__C9F284566237D173")
                    .IsUnique();

                entity.Property(e => e.AllowedIpaddress)
                    .HasMaxLength(255)
                    .HasColumnName("AllowedIPAddress");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UserCreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserPhone).HasMaxLength(20);

                entity.Property(e => e.UserUpdatedAt).HasDefaultValueSql("(sysdatetime())");
            });

            modelBuilder.Entity<AdminUserRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__AdminUse__8AFACE1AC4D71493");

                entity.HasIndex(e => e.RoleName, "UQ__AdminUse__8A2B61600ECEDBC5")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<AirlineCodeShare>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("airline_codeShare");

                entity.Property(e => e.AirlineCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("airlineCode")
                    .IsFixedLength(true);

                entity.Property(e => e.AirlineCodeCodeShare)
                    .HasMaxLength(2000)
                    .HasColumnName("airlineCode_codeShare");

                entity.Property(e => e.Destination)
                    .IsUnicode(false)
                    .HasColumnName("destination");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Origin)
                    .IsUnicode(false)
                    .HasColumnName("origin");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<AirlineDetail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AirlineCode).HasMaxLength(50);

                entity.Property(e => e.AirlineImage).HasMaxLength(100);

                entity.Property(e => e.AirlineImage1).HasMaxLength(200);

                entity.Property(e => e.AirlineImage2).HasMaxLength(200);

                entity.Property(e => e.AirlineImagePath).HasMaxLength(200);

                entity.Property(e => e.AirlineName).HasMaxLength(50);

                entity.Property(e => e.Business).HasMaxLength(200);

                entity.Property(e => e.Economy).HasMaxLength(200);

                entity.Property(e => e.First).HasMaxLength(200);

                entity.Property(e => e.GeneralInformation).HasMaxLength(3000);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PremiumEconomy).HasMaxLength(200);
            });

            modelBuilder.Entity<AirlineRoute>(entity =>
            {
                entity.HasKey(e => e.RoutesId)
                    .HasName("PK__AirlineR__6CDC2B43DC9A2043");

                entity.Property(e => e.CampId)
                    .HasMaxLength(50)
                    .HasColumnName("CampID");

                entity.Property(e => e.Company).HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedAt).HasPrecision(0);

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            });

            modelBuilder.Entity<AirlineTable>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Airline_Table");

                entity.Property(e => e.AirlineCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Airline Code");

                entity.Property(e => e.AirlineImageName).HasMaxLength(100);

                entity.Property(e => e.AirlineName)
                    .HasMaxLength(50)
                    .HasColumnName("Airline Name");

                entity.Property(e => e.Imagepath).HasMaxLength(200);

                entity.Property(e => e.VideoPath).HasMaxLength(2000);

                entity.Property(e => e.Visible).HasColumnName("visible");
            });

            modelBuilder.Entity<AirlineTable1>(entity =>
            {
                entity.ToTable("AirlineTable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AirlineCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.AirlineName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Apicredential>(entity =>
            {
                entity.ToTable("APICredential");

                entity.Property(e => e.ApicredentialId).HasColumnName("APICredentialID");

                entity.Property(e => e.AllCredentials)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Apicode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APICode");

                entity.Property(e => e.Apiname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APIName");

                entity.Property(e => e.Apipass)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("APIPass");

                entity.Property(e => e.Apistatus)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APIStatus")
                    .HasComment("Test, Live");

                entity.Property(e => e.Apiswitch)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APISwitch");

                entity.Property(e => e.Apitype)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APIType");

                entity.Property(e => e.Apiurl)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("APIUrl");

                entity.Property(e => e.ApiuserId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APIUserID");

                entity.Property(e => e.ApiuserName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("APIUserName");

                entity.Property(e => e.ConsolidatorId).HasColumnName("ConsolidatorID");

                entity.Property(e => e.CredentialFormat)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalIpcc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<ApplyCoupon>(entity =>
            {
                entity.ToTable("ApplyCoupon");

                entity.Property(e => e.Code).HasMaxLength(25);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdDate");

                entity.Property(e => e.DestinationCode)
                    .HasMaxLength(10)
                    .HasColumnName("destinationCode");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.ImageUrl).HasMaxLength(100);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.Property(e => e.ValueType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("valueType");

                entity.Property(e => e.WebsiteId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteID");
            });

            modelBuilder.Entity<BannerDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BannerDetail");

                entity.HasComment("may be move this functionality to websiteManagement DB");

                entity.Property(e => e.AirlineText).HasMaxLength(4000);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Image).HasMaxLength(50);

                entity.Property(e => e.PageType).HasMaxLength(50);

                entity.Property(e => e.UrlAddress).HasMaxLength(500);

                entity.Property(e => e.WebsiteId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("WebsiteID");
            });

            modelBuilder.Entity<BlockAirlineRule>(entity =>
            {
                entity.HasKey(e => e.RuleId)
                    .HasName("PK__BlockAir__110458E2601DE4EB");

                entity.Property(e => e.BlockDates).HasMaxLength(100);

                entity.Property(e => e.CabinClassIb)
                    .HasMaxLength(20)
                    .HasColumnName("CabinClassIB");

                entity.Property(e => e.CabinClassOb)
                    .HasMaxLength(20)
                    .HasColumnName("CabinClassOB");

                entity.Property(e => e.CampId)
                    .HasMaxLength(50)
                    .HasColumnName("CampID");

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(CONVERT([date],getdate()))");

                entity.Property(e => e.DateFrom).HasColumnType("date");

                entity.Property(e => e.DateTo).HasColumnType("date");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TimeIb)
                    .HasMaxLength(30)
                    .HasColumnName("TimeIB");

                entity.Property(e => e.TimeOb)
                    .HasMaxLength(30)
                    .HasColumnName("TimeOB");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            });

            modelBuilder.Entity<BlockBaggageMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BlockBaggageMaster");

                entity.Property(e => e.Airline)
                    .HasMaxLength(300)
                    .HasColumnName("airline");

                entity.Property(e => e.BookingCode)
                    .HasMaxLength(50)
                    .HasColumnName("bookingCode");

                entity.Property(e => e.Destination)
                    .HasMaxLength(600)
                    .HasColumnName("destination");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Origin)
                    .HasMaxLength(600)
                    .HasColumnName("origin");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<BlockedAirline>(entity =>
            {
                entity.ToTable("blockedAirlines");

                entity.Property(e => e.Airlinecodes)
                    .HasMaxLength(4000)
                    .HasColumnName("airlinecodes");

                entity.Property(e => e.Destination)
                    .HasMaxLength(3)
                    .HasColumnName("destination")
                    .IsFixedLength(true);

                entity.Property(e => e.Origin)
                    .HasMaxLength(3)
                    .HasColumnName("origin")
                    .IsFixedLength(true);

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Blog");

                entity.Property(e => e.BlogId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("blogID");

                entity.Property(e => e.DescriptionFile)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("descriptionFile");

                entity.Property(e => e.Imagefile)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SystemDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Websiteid)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("websiteid");
            });

            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.Property(e => e.AltImageText)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ContentType).HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EditedBy).HasMaxLength(250);

                entity.Property(e => e.EditedDate).HasColumnType("datetime");

                entity.Property(e => e.FeatureImageUrl).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeywords).HasMaxLength(1000);

                entity.Property(e => e.MetaTitle).HasMaxLength(255);

                entity.Property(e => e.Slug)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SocialProfileUrl).HasMaxLength(1000);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SubHeading).HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.WebsiteId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<BlogReview>(entity =>
            {
                entity.HasKey(e => e.ReviewId)
                    .HasName("PK__BlogRevi__74BC79CED362429C");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ReviewStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Pending')");

                entity.Property(e => e.ReviewerComment).IsRequired();

                entity.Property(e => e.ReviewerEmail)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ReviewerName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookQuoteMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BookQuoteMaster");

                entity.Property(e => e.BookId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("bookId");

                entity.Property(e => e.BookRefNo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("bookRefNo");

                entity.Property(e => e.DepartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("departDate");

                entity.Property(e => e.EmailId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("emailId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserHit)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userHit");

                entity.Property(e => e.WebsiteId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bookings");

                entity.Property(e => e.AddCard)
                    .HasColumnName("add_card")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Adult)
                    .HasColumnName("adult")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Airline)
                    .HasMaxLength(20)
                    .HasColumnName("airline");

                entity.Property(e => e.CabinClass)
                    .HasMaxLength(50)
                    .HasColumnName("cabin_class");

                entity.Property(e => e.Children)
                    .HasColumnName("children")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DepartureDate)
                    .HasMaxLength(50)
                    .HasColumnName("departure_date");

                entity.Property(e => e.Destination)
                    .HasMaxLength(100)
                    .HasColumnName("destination");

                entity.Property(e => e.EmailId)
                    .HasMaxLength(100)
                    .HasColumnName("email_id");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Infrants)
                    .HasColumnName("infrants")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.RequestUrl)
                    .HasMaxLength(500)
                    .HasColumnName("request_url");

                entity.Property(e => e.ReturnDate)
                    .HasMaxLength(50)
                    .HasColumnName("return_date");

                entity.Property(e => e.SourceStation)
                    .HasMaxLength(20)
                    .HasColumnName("source_station");

                entity.Property(e => e.TotalFare)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("total_fare");

                entity.Property(e => e.Website)
                    .HasMaxLength(200)
                    .HasColumnName("website");
            });

            modelBuilder.Entity<BookingConversation>(entity =>
            {
                entity.ToTable("BookingConversation");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.SenderName).HasMaxLength(100);

                entity.Property(e => e.SenderRole).HasMaxLength(50);
            });

            modelBuilder.Entity<BookingFlghtDetail>(entity =>
            {
                entity.HasKey(e => e.FlightSegmentId)
                    .HasName("PK__BookingF__AA88C0544721F14D");

                entity.ToTable("BookingFlghtDetail");

                entity.Property(e => e.Airline).HasMaxLength(100);

                entity.Property(e => e.ArrDate).HasColumnType("datetime");

                entity.Property(e => e.ArrivalDate).HasMaxLength(50);

                entity.Property(e => e.ArriveOn).HasMaxLength(100);

                entity.Property(e => e.BoundClass).HasMaxLength(50);

                entity.Property(e => e.Dcode).HasMaxLength(10);

                entity.Property(e => e.DepDate).HasColumnType("datetime");

                entity.Property(e => e.DepartDate).HasMaxLength(50);

                entity.Property(e => e.DepartOn).HasMaxLength(100);

                entity.Property(e => e.FlightNo).HasMaxLength(50);

                entity.Property(e => e.FlightType).HasMaxLength(50);

                entity.Property(e => e.Gcode).HasMaxLength(10);
            });

            modelBuilder.Entity<BookingPaxDetail>(entity =>
            {
                entity.HasKey(e => e.PaxId)
                    .HasName("PK__BookingP__F77856E10966C41C");

                entity.ToTable("BookingPaxDetail");

                entity.Property(e => e.Dob).HasMaxLength(20);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.Gender).HasMaxLength(20);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.MiddleName).HasMaxLength(100);

                entity.Property(e => e.PaxTitle).HasMaxLength(50);

                entity.Property(e => e.PaxType).HasMaxLength(50);
            });

            modelBuilder.Entity<BookingsB2c>(entity =>
            {
                entity.HasKey(e => e.BookingId)
                    .HasName("PK__Bookings__73951AED23F3538A");

                entity.ToTable("Bookings_B2C");

                entity.Property(e => e.Bookingstatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Destination)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Emailid)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Origin)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.PaylaterUrl).HasMaxLength(500);

                entity.Property(e => e.PnrNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RandomNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SystemDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPax)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.TravelDateFrom).HasColumnType("datetime");

                entity.Property(e => e.TravelDateTo).HasColumnType("datetime");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("WebsiteID");
            });

            modelBuilder.Entity<BookingsCardDetail>(entity =>
            {
                entity.ToTable("bookings_cardDetails");

                entity.Property(e => e.BookingsId).HasColumnName("bookings_Id");

                entity.Property(e => e.CardNumber)
                    .HasMaxLength(50)
                    .HasColumnName("card_number");

                entity.Property(e => e.CardType)
                    .HasMaxLength(50)
                    .HasColumnName("card_type");

                entity.Property(e => e.Cvc)
                    .HasMaxLength(10)
                    .HasColumnName("cvc");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpiryDate)
                    .HasMaxLength(10)
                    .HasColumnName("expiry_date");

                entity.Property(e => e.ExpiryYear)
                    .HasMaxLength(10)
                    .HasColumnName("expiry_year");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(100)
                    .HasColumnName("phone");

                entity.Property(e => e.Url)
                    .HasMaxLength(200)
                    .HasColumnName("url");

                entity.Property(e => e.Website)
                    .HasMaxLength(200)
                    .HasColumnName("website");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<CacheGd>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Cache_Gds");

                entity.Property(e => e.Adults).HasColumnName("adults");

                entity.Property(e => e.Children).HasColumnName("children");

                entity.Property(e => e.Class)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.DepDate)
                    .HasMaxLength(20)
                    .HasColumnName("depDate");

                entity.Property(e => e.Destination)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("destination")
                    .IsFixedLength(true);

                entity.Property(e => e.GdsType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("gdsType")
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Origin)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("origin")
                    .IsFixedLength(true);

                entity.Property(e => e.RetDate)
                    .HasMaxLength(20)
                    .HasColumnName("retDate");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserHit)
                    .HasMaxLength(100)
                    .HasColumnName("userHit");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(30)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.Code)
                    .HasMaxLength(3)
                    .HasColumnName("code")
                    .IsFixedLength(true);

                entity.Property(e => e.IsCart).HasColumnName("isCart");

                entity.Property(e => e.Service)
                    .HasMaxLength(50)
                    .HasColumnName("service");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteID")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<CityGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CityGroup");

                entity.Property(e => e.CityCode).HasColumnName("cityCode");

                entity.Property(e => e.CityGroupName)
                    .HasMaxLength(20)
                    .HasColumnName("cityGroupName");

                entity.Property(e => e.GroupType)
                    .HasMaxLength(15)
                    .HasColumnName("groupType");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SaAccountNo)
                    .HasMaxLength(20)
                    .HasColumnName("Sa_AccountNo");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<CityMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CityMaster");

                entity.Property(e => e.CityCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CityId).ValueGeneratedOnAdd();

                entity.Property(e => e.CityName).HasMaxLength(50);

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Image).HasMaxLength(100);

                entity.Property(e => e.IsFeatured).HasColumnName("isFeatured");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Summary).HasMaxLength(4000);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<Continent>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Continent");

                entity.Property(e => e.ContinentImage)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("continent_image");

                entity.Property(e => e.ContinentName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("continentName");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("country");

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("countryCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsFeatured).HasColumnName("isFeatured");

                entity.Property(e => e.SaAccountNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Sa_AccountNo");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<CopyOffersWebsite>(entity =>
            {
                entity.ToTable("CopyOffersWebsite");

                entity.Property(e => e.CopyFromWebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("copyFrom_websiteID");

                entity.Property(e => e.CopyToWebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("copyTo_websiteID");

                entity.Property(e => e.CreatedBy).HasMaxLength(25);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CouponMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CouponMaster");

                entity.Property(e => e.ActiveFromDate).HasColumnType("datetime");

                entity.Property(e => e.ActiveTillDate).HasColumnType("datetime");

                entity.Property(e => e.CouponCode).HasMaxLength(50);

                entity.Property(e => e.CouponValue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.MinValue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebsiteId).HasMaxLength(20);
            });

            modelBuilder.Entity<CurrencyMaster>(entity =>
            {
                entity.ToTable("currencyMaster");

                entity.Property(e => e.BaseCurrency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("baseCurrency")
                    .IsFixedLength(true);

                entity.Property(e => e.ChangeCurrency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("changeCurrency")
                    .IsFixedLength(true);

                entity.Property(e => e.CurrencyValue).HasColumnName("currencyValue");

                entity.Property(e => e.WebsiteId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<DeepLinkTracker>(entity =>
            {
                entity.Property(e => e.Adults).HasDefaultValueSql("((1))");

                entity.Property(e => e.Airline)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BookingReference).HasMaxLength(100);

                entity.Property(e => e.CabinClass).HasMaxLength(10);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.DepartureDate).HasColumnType("date");

                entity.Property(e => e.FromLocation).HasMaxLength(10);

                entity.Property(e => e.InboundAirline).HasMaxLength(100);

                entity.Property(e => e.InboundClass).HasMaxLength(10);

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(50)
                    .HasColumnName("IPAddress");

                entity.Property(e => e.Markup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OutboundAirline).HasMaxLength(100);

                entity.Property(e => e.OutboundClass).HasMaxLength(10);

                entity.Property(e => e.ParentSearchId).HasMaxLength(64);

                entity.Property(e => e.PreviousStage).HasMaxLength(50);

                entity.Property(e => e.PriceWithoutMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ReturnDate).HasColumnType("date");

                entity.Property(e => e.RootSource)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Rule)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName(" Rule");

                entity.Property(e => e.SearchId)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Stage)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SubClass)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubDetailStage)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToLocation).HasMaxLength(10);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TripType).HasMaxLength(5);
            });

            modelBuilder.Entity<DestinationAirportMapping>(entity =>
            {
                entity.ToTable("destinationAirportMapping");

                entity.Property(e => e.AirPort)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Destination)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DestinationFareType>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("destination_fareType");

                entity.Property(e => e.DestinationCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("destinationCode")
                    .IsFixedLength(true);

                entity.Property(e => e.FareType)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("fareType")
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<DynamicPage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("dynamicPages");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Images)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("images");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ServiceId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("serviceId");

                entity.Property(e => e.ServicePageName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("servicePageName");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<DynamicQueryForm>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Dynamic_queryForm");

                entity.Property(e => e.ControlName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("control_name");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Type)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Websiteid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteid");
            });

            modelBuilder.Entity<DynamicQueryForm1>(entity =>
            {
                entity.ToTable("dynamicQueryForm");

                entity.Property(e => e.ControlName)
                    .HasMaxLength(200)
                    .HasColumnName("controlName");

                entity.Property(e => e.ControlType)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("controlType")
                    .IsFixedLength(true);

                entity.Property(e => e.FormHeading)
                    .HasMaxLength(500)
                    .HasColumnName("formHeading");

                entity.Property(e => e.Heading)
                    .HasMaxLength(500)
                    .HasColumnName("heading");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Page)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("page");

                entity.Property(e => e.Sequence).HasColumnName("sequence");

                entity.Property(e => e.SubHeading)
                    .HasMaxLength(500)
                    .HasColumnName("subHeading");

                entity.Property(e => e.Type)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("type")
                    .IsFixedLength(true);

                entity.Property(e => e.Websiteid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteid");
            });

            modelBuilder.Entity<EquipmentAir>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EquipmentAir");

                entity.HasComment("move to DataMaster db");

                entity.Property(e => e.Iata)
                    .HasMaxLength(10)
                    .HasColumnName("IATA");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Manufacturer).HasMaxLength(20);

                entity.Property(e => e.Type).HasMaxLength(100);

                entity.Property(e => e.Wake).HasMaxLength(5);
            });

            modelBuilder.Entity<ExcelUploadStatusLog>(entity =>
            {
                entity.ToTable("ExcelUploadStatusLog");

                entity.Property(e => e.FileName).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UploadedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<ExternalClicksMetaService>(entity =>
            {
                entity.HasKey(e => e.UserHit)
                    .HasName("PK_ExternalClicks");

                entity.ToTable("ExternalClicks_MetaService");

                entity.Property(e => e.UserHit).HasMaxLength(50);

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.BookingRef).HasColumnName("booking_ref");

                entity.Property(e => e.CabinClass).HasMaxLength(20);

                entity.Property(e => e.DepartDate).HasColumnType("datetime");

                entity.Property(e => e.Destination)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.JType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("jType")
                    .IsFixedLength(true);

                entity.Property(e => e.Origin)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ReturnDate).HasColumnType("datetime");

                entity.Property(e => e.Source)
                    .HasMaxLength(50)
                    .HasColumnName("source");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.UserIpadress)
                    .HasMaxLength(20)
                    .HasColumnName("User_IPadress");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<ExtraService>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("extraServices");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.DestinationCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("destinationCode");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.ServiceDescription)
                    .HasMaxLength(4000)
                    .HasColumnName("serviceDescription");

                entity.Property(e => e.ServiceId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("serviceId");

                entity.Property(e => e.ServiceName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("serviceName");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<Faq>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Faq");

                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasMaxLength(5000)
                    .IsUnicode(false)
                    .HasColumnName("answer");

                entity.Property(e => e.FaqId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("faqId");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("question");

                entity.Property(e => e.SearchTags)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("searchTags");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebsiteId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<FaqMaster>(entity =>
            {
                entity.HasKey(e => e.FaqId)
                    .HasName("PK__FaqMaste__9C741C438B6A479E");

                entity.ToTable("FaqMaster");

                entity.HasIndex(e => new { e.ReferenceType, e.ReferenceId, e.Delind, e.IsActive }, "IX_FaqMaster_Reference");

                entity.Property(e => e.Answer).IsRequired();

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ReferenceType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FareInfoNego>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("fareInfo_Nego");

                entity.Property(e => e.AdtFare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("adtFare");

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("airline")
                    .IsFixedLength(true);

                entity.Property(e => e.Arrive)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("arrive")
                    .IsFixedLength(true);

                entity.Property(e => e.Cabin)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cabin")
                    .IsFixedLength(true);

                entity.Property(e => e.ChdFare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("chdFare");

                entity.Property(e => e.Class)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("class")
                    .IsFixedLength(true);

                entity.Property(e => e.CrDt)
                    .HasColumnType("datetime")
                    .HasColumnName("cr_dt");

                entity.Property(e => e.Depart)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("depart")
                    .IsFixedLength(true);

                entity.Property(e => e.FareBasic)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("fareBasic")
                    .IsFixedLength(true);

                entity.Property(e => e.Ftype)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ftype")
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("id")
                    .IsFixedLength(true);

                entity.Property(e => e.InfFare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("infFare");

                entity.Property(e => e.LastDt)
                    .HasColumnType("datetime")
                    .HasColumnName("last_dt");

                entity.Property(e => e.RuleKey).HasMaxLength(500);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<Fareinfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("fareinfo");

                entity.Property(e => e.AdtFare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("adtFare");

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("airline")
                    .IsFixedLength(true);

                entity.Property(e => e.Arrive)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("arrive")
                    .IsFixedLength(true);

                entity.Property(e => e.Cabin)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cabin")
                    .IsFixedLength(true);

                entity.Property(e => e.ChdFare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("chdFare");

                entity.Property(e => e.Class)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("class")
                    .IsFixedLength(true);

                entity.Property(e => e.CrDt)
                    .HasColumnType("datetime")
                    .HasColumnName("cr_dt");

                entity.Property(e => e.Depart)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("depart")
                    .IsFixedLength(true);

                entity.Property(e => e.DepartDate).HasColumnType("datetime");

                entity.Property(e => e.FareBasic)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("fareBasic")
                    .IsFixedLength(true);

                entity.Property(e => e.Ftype)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ftype")
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("id")
                    .IsFixedLength(true);

                entity.Property(e => e.InfFare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("infFare");

                entity.Property(e => e.LastDt)
                    .HasColumnType("datetime")
                    .HasColumnName("last_dt");

                entity.Property(e => e.ReturnDate).HasColumnType("datetime");

                entity.Property(e => e.RuleKey).HasMaxLength(500);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");

                entity.Property(e => e.YthFare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ythFare");
            });

            modelBuilder.Entity<FareinfoMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("fareinfo_Master");

                entity.HasComment("add websiteId col  , rename to fareInfo");

                entity.Property(e => e.AdtYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("adtYQ");

                entity.Property(e => e.Adtfare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("adtfare");

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("airline")
                    .IsFixedLength(true);

                entity.Property(e => e.Arrive)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("arrive")
                    .IsFixedLength(true);

                entity.Property(e => e.Cabin)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cabin")
                    .IsFixedLength(true);

                entity.Property(e => e.ChdYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("chdYQ");

                entity.Property(e => e.Chdfare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("chdfare");

                entity.Property(e => e.Class)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("class")
                    .IsFixedLength(true);

                entity.Property(e => e.CrDt)
                    .HasColumnType("datetime")
                    .HasColumnName("cr_dt");

                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("currency")
                    .IsFixedLength(true);

                entity.Property(e => e.Depart)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("depart")
                    .IsFixedLength(true);

                entity.Property(e => e.Ftype)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ftype")
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("id")
                    .IsFixedLength(true);

                entity.Property(e => e.InfYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("infYQ");

                entity.Property(e => e.Inffare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("inffare");

                entity.Property(e => e.LastDt)
                    .HasColumnType("datetime")
                    .HasColumnName("last_dt");

                entity.Property(e => e.NdAdtYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_adtYQ");

                entity.Property(e => e.NdAdtYq3)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_adtYQ_3");

                entity.Property(e => e.NdAdtfare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_adtfare");

                entity.Property(e => e.NdAdtfare3)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_adtfare_3");

                entity.Property(e => e.NdChdYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_chdYQ");

                entity.Property(e => e.NdChdYq3)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_chdYQ_3");

                entity.Property(e => e.NdChdfare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_chdfare");

                entity.Property(e => e.NdChdfare3)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_chdfare_3");

                entity.Property(e => e.NdInfYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_infYQ");

                entity.Property(e => e.NdInfYq3)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_infYQ_3");

                entity.Property(e => e.NdInffare)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_inffare");

                entity.Property(e => e.NdInffare3)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_inffare_3");
            });

            modelBuilder.Entity<FeatureFlight>(entity =>
            {
                entity.ToTable("FeatureFlight");

                entity.Property(e => e.AirlineCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Destination)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Origin)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.SystemDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FeaturedAirline>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FeaturedAirline");

                entity.Property(e => e.AirCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("airCode")
                    .IsFixedLength(true);

                entity.Property(e => e.AirName)
                    .HasMaxLength(20)
                    .HasColumnName("airName");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<FlightBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId)
                    .HasName("PK__FlightBo__73951AED5DD47B19");

                entity.ToTable("FlightBooking");

                entity.Property(e => e.AdultFare).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AdultMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AdutlTax).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Agent).HasMaxLength(100);

                entity.Property(e => e.Airline).HasMaxLength(100);

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AssignDate).HasColumnType("datetime");

                entity.Property(e => e.BaggageCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BaggageSize).HasMaxLength(100);

                entity.Property(e => e.BillAddress1).HasMaxLength(200);

                entity.Property(e => e.BillAddress2).HasMaxLength(200);

                entity.Property(e => e.BillCity).HasMaxLength(100);

                entity.Property(e => e.BillCountry).HasMaxLength(100);

                entity.Property(e => e.BillPostalCode).HasMaxLength(50);

                entity.Property(e => e.BillState).HasMaxLength(100);

                entity.Property(e => e.BookingDate).HasColumnType("datetime");

                entity.Property(e => e.BookingNumber).HasMaxLength(50);

                entity.Property(e => e.CardCharge).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CardName).HasMaxLength(100);

                entity.Property(e => e.CardNumber).HasMaxLength(50);

                entity.Property(e => e.CardSecCode).HasMaxLength(10);

                entity.Property(e => e.CardType).HasMaxLength(50);

                entity.Property(e => e.CardValMonth).HasMaxLength(10);

                entity.Property(e => e.ChildFare).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ChildMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ChildTax).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ContactEmail).HasMaxLength(100);

                entity.Property(e => e.ContactPerson).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Currency).HasMaxLength(10);

                entity.Property(e => e.DepartDate).HasMaxLength(50);

                entity.Property(e => e.FlightClass).HasMaxLength(50);

                entity.Property(e => e.FlightCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FlightNo).HasMaxLength(50);

                entity.Property(e => e.FlightWay).HasMaxLength(50);

                entity.Property(e => e.FromDeparture).HasMaxLength(50);

                entity.Property(e => e.Gross).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InfantFare).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InfantMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InfantTax).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Initialmco).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Ipaddress).HasMaxLength(50);

                entity.Property(e => e.IsStatus).HasMaxLength(50);

                entity.Property(e => e.Marketing).HasMaxLength(100);

                entity.Property(e => e.MarkupType).HasMaxLength(50);

                entity.Property(e => e.MarkupValue).HasMaxLength(50);

                entity.Property(e => e.Mco).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MercahntFeeAfterCal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MerchantFeeInPercent).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Nett).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OurFare).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PassengerTitle)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PaxFullName).HasMaxLength(100);

                entity.Property(e => e.PaymentType).HasMaxLength(50);

                entity.Property(e => e.PhoneCountry)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Profitt).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Provider).HasMaxLength(100);

                entity.Property(e => e.ReturnDate).HasMaxLength(50);

                entity.Property(e => e.SearchId).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.SupplierFee).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SupportCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SupportPlan).HasMaxLength(100);

                entity.Property(e => e.ToDestination).HasMaxLength(50);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalFare).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalMco).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<FlightSearchCache>(entity =>
            {
                entity.HasKey(e => e.SearchKey)
                    .HasName("PK__FlightSe__4174EA4EE2AE120D");

                entity.ToTable("FlightSearchCache");

                entity.Property(e => e.CacheKey)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Destination)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.Origin)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SearchId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<GoogleOauthToken>(entity =>
            {
                entity.ToTable("GoogleOAuthTokens");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RefreshToken).IsRequired();

                entity.Property(e => e.Scope).HasMaxLength(500);

                entity.Property(e => e.TokenType).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HolidayDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("HolidayDetail");

                entity.Property(e => e.HolidayText).HasMaxLength(4000);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Image1).HasMaxLength(50);

                entity.Property(e => e.Image2).HasMaxLength(50);

                entity.Property(e => e.WebsiteId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("WebsiteID");
            });

            modelBuilder.Entity<HomeOffersCoupon>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("home_OffersCoupon");

                entity.Property(e => e.Code).HasMaxLength(25);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdDate");

                entity.Property(e => e.DestinationCode)
                    .HasMaxLength(10)
                    .HasColumnName("destinationCode");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ImageUrl).HasMaxLength(100);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.Property(e => e.ValueType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("valueType");

                entity.Property(e => e.WebsiteId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteID");
            });

            modelBuilder.Entity<HomePackageCoupon>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("HomePackageCoupon");

                entity.Property(e => e.Code).HasMaxLength(25);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ImageUrl).HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PackageTitle).HasMaxLength(100);

                entity.Property(e => e.PageUrl)
                    .HasMaxLength(300)
                    .HasColumnName("PageURL");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.Property(e => e.ValueType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("valueType")
                    .IsFixedLength(true);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteID");
            });

            modelBuilder.Entity<HomeSequence>(entity =>
            {
                entity.ToTable("home_Sequence");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ControlName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("controlName");

                entity.Property(e => e.Sequence).HasColumnName("sequence");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.Websiteid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteid");
            });

            modelBuilder.Entity<HotelMarkupDetail>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.BookingFromDate).HasColumnType("datetime");

                entity.Property(e => e.BookingToDate).HasColumnType("datetime");

                entity.Property(e => e.CheckInDate).HasColumnType("datetime");

                entity.Property(e => e.CheckOutDate).HasColumnType("datetime");

                entity.Property(e => e.City)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Country)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Markup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MarkupType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("markupType")
                    .IsFixedLength(true);

                entity.Property(e => e.SaAccountNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Sa_AccountNo");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HotelMasterSelected>(entity =>
            {
                entity.ToTable("HotelMaster_Selected");

                entity.Property(e => e.HotelName).HasMaxLength(200);

                entity.Property(e => e.RegionId).HasMaxLength(20);

                entity.Property(e => e.SupplierCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InOutAirlineMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("InOutAirlineMaster");

                entity.Property(e => e.DepartAirlineCode)
                    .HasMaxLength(2)
                    .HasColumnName("departAirlineCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ReturnAirlineCode)
                    .HasMaxLength(20)
                    .HasColumnName("returnAirlineCode");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<InclusionExclusion>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("InclusionExclusion");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.Text).HasMaxLength(4000);

                entity.Property(e => e.Type)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<InsuranceOffline>(entity =>
            {
                entity.ToTable("Insurance_Offline");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("amount");

                entity.Property(e => e.AmountType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("amountType");

                entity.Property(e => e.DestinationCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("destinationCode");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.ValidTill)
                    .HasColumnType("datetime")
                    .HasColumnName("validTill");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("websiteID");
            });

            modelBuilder.Entity<LikedFareBasis>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Liked_FareBasis");

                entity.Property(e => e.AirCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("airCode")
                    .IsFixedLength(true);

                entity.Property(e => e.FareBasis)
                    .HasMaxLength(20)
                    .HasColumnName("fareBasis");

                entity.Property(e => e.HitCount).HasColumnName("hitCount");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");
            });

            modelBuilder.Entity<Login>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Login");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.Property(e => e.UserType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<ManageFooter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ManageFooter");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Keyword)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Link)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LogoName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Websiteid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteid");
            });

            modelBuilder.Entity<ManageHeader>(entity =>
            {
                entity.ToTable("ManageHeader");

                entity.Property(e => e.Keyword).HasMaxLength(50);

                entity.Property(e => e.Link).HasMaxLength(100);

                entity.Property(e => e.LogoName).HasMaxLength(50);

                entity.Property(e => e.Websiteid)
                    .HasMaxLength(50)
                    .HasColumnName("websiteid");
            });

            modelBuilder.Entity<ManageMenu>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ManageMenu");

                entity.Property(e => e.Icon)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("icon");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsExternal).HasDefaultValueSql("((0))");

                entity.Property(e => e.MenuLink)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MenuName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Websiteid)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ManageUsp>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ManageUsp");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Image).HasMaxLength(150);

                entity.Property(e => e.SystemDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Websiteid)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("websiteid");
            });

            modelBuilder.Entity<MarkupDetail>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.CabinClass, e.Destination, e.FromCity, e.ToCity, e.SaAccountNo, e.TravelDateFrom, e.TravelDateTo }, "<24jul24_1>");

                entity.HasIndex(e => e.Id, "<24jul24_2>");

                entity.HasIndex(e => new { e.SaAccountNo, e.CabinClass, e.Destination, e.FromCity, e.ToCity, e.TravelDateFrom, e.TravelDateTo, e.DepartCountry }, "missing_index_10_9");

                entity.HasIndex(e => new { e.SaAccountNo, e.CabinClass, e.Destination, e.FromCity, e.ToCity, e.DepartCountry }, "missing_index_12_11");

                entity.HasIndex(e => new { e.SaAccountNo, e.IsActive, e.CabinClass, e.Destination, e.FromCity, e.ToCity, e.DepartCountry }, "missing_index_14_13");

                entity.HasIndex(e => new { e.SaAccountNo, e.IsActive }, "missing_index_35_34");

                entity.HasIndex(e => e.SaAccountNo, "missing_index_53_52");

                entity.HasIndex(e => new { e.SaAccountNo, e.StartingPrice }, "missing_index_55_54");

                entity.HasIndex(e => new { e.Airline, e.SaAccountNo }, "missing_index_59_58");

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.BookingClass).HasMaxLength(50);

                entity.Property(e => e.CabinClass)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CityGroup)
                    .HasMaxLength(50)
                    .HasColumnName("cityGroup");

                entity.Property(e => e.Continent)
                    .HasMaxLength(50)
                    .HasColumnName("continent");

                entity.Property(e => e.DepartCountry).HasMaxLength(50);

                entity.Property(e => e.DepartDurationCondition)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.DepartStopOverCondition)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.Destination).HasMaxLength(50);

                entity.Property(e => e.EndingPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FareBasis)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.FareType)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.From).HasMaxLength(50);

                entity.Property(e => e.FromCity)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsRecursive)
                    .HasColumnName("isRecursive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.JType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("jType")
                    .IsFixedLength(true);

                entity.Property(e => e.Markup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MarkupType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("markupType")
                    .IsFixedLength(true);

                entity.Property(e => e.OriginCityGroup).HasMaxLength(50);

                entity.Property(e => e.PaxCondition)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.PaxType)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.RAirline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("R_Airline")
                    .IsFixedLength(true);

                entity.Property(e => e.ReturnBookingClass)
                    .HasMaxLength(50)
                    .HasColumnName("returnBookingClass");

                entity.Property(e => e.ReturnDurationCondition)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.ReturnStopOverCondition)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.ReturnTravelDateFrom).HasColumnType("datetime");

                entity.Property(e => e.ReturnTravelDateTo).HasColumnType("datetime");

                entity.Property(e => e.SaAccountNo)
                    .HasMaxLength(20)
                    .HasColumnName("Sa_AccountNo");

                entity.Property(e => e.SearchDateFrom)
                    .HasColumnType("datetime")
                    .HasColumnName("searchDateFrom");

                entity.Property(e => e.SearchDateTo)
                    .HasColumnType("datetime")
                    .HasColumnName("searchDateTo");

                entity.Property(e => e.StartingPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ToCity)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TravelDateFrom).HasColumnType("datetime");

                entity.Property(e => e.TravelDateTo).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarkupDetailsAdvance>(entity =>
            {
                entity.ToTable("markupDetails_Advance");

                entity.Property(e => e.MarkAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("markAmount");

                entity.Property(e => e.MarkupDetailsId).HasColumnName("markupDetailsId");

                entity.Property(e => e.PaxCount).HasColumnName("paxCount");

                entity.Property(e => e.PaxType)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("paxType")
                    .IsFixedLength(true);

                entity.Property(e => e.SplCondition)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("splCondition")
                    .IsFixedLength(true);

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<MarkupDetailsHotel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MarkupDetails_Hotel");

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.BookingClass)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CabinClass)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Destination).HasMaxLength(50);

                entity.Property(e => e.FareBasis)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.FareType)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.From).HasMaxLength(50);

                entity.Property(e => e.FromCity)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsRecursive)
                    .HasColumnName("isRecursive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Markup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MarkupType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("markupType")
                    .IsFixedLength(true);

                entity.Property(e => e.PaxType)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.PerNight).HasDefaultValueSql("((0))");

                entity.Property(e => e.PerRoom).HasDefaultValueSql("((0))");

                entity.Property(e => e.SaAccountNo)
                    .HasMaxLength(20)
                    .HasColumnName("Sa_AccountNo");

                entity.Property(e => e.SearchDateFrom)
                    .HasColumnType("datetime")
                    .HasColumnName("searchDateFrom");

                entity.Property(e => e.SearchDateTo)
                    .HasColumnType("datetime")
                    .HasColumnName("searchDateTo");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ToCity)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TravelDateFrom).HasColumnType("datetime");

                entity.Property(e => e.TravelDateTo).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarkupGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("markupGroup");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.MarkupDetailId)
                    .HasMaxLength(50)
                    .HasColumnName("MarkupDetail_Id");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.Property(e => e.WebsiteMaterId)
                    .HasMaxLength(50)
                    .HasColumnName("WebsiteMater_Id");
            });

            modelBuilder.Entity<MarkupGroupMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("markup_GroupMaster");

                entity.Property(e => e.AirlineGroup).HasMaxLength(1000);

                entity.Property(e => e.GroupName).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<MarkupMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Markup_Master");

                entity.Property(e => e.AirlineCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Markup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MarkupId).ValueGeneratedOnAdd();

                entity.Property(e => e.SaAccountNo).HasMaxLength(50);

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<MarkupRule>(entity =>
            {
                entity.HasKey(e => e.RuleId)
                    .HasName("PK__MarkupRu__110458E2EFBAFB2C");

                entity.Property(e => e.BlockOdateFrom)
                    .HasColumnType("date")
                    .HasColumnName("BlockODateFrom");

                entity.Property(e => e.BlockOdateTo)
                    .HasColumnType("date")
                    .HasColumnName("BlockODateTo");

                entity.Property(e => e.CabinClass).HasMaxLength(20);

                entity.Property(e => e.CampId).HasMaxLength(100);

                entity.Property(e => e.ClassIb)
                    .HasMaxLength(100)
                    .HasColumnName("ClassIB");

                entity.Property(e => e.ClassOb)
                    .HasMaxLength(100)
                    .HasColumnName("ClassOB");

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.JourneyType).HasMaxLength(10);

                entity.Property(e => e.MarkApplicable2).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MarkType).HasMaxLength(1);

                entity.Property(e => e.MarkUp).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MinimumAmount).HasMaxLength(50);

                entity.Property(e => e.Pairline).HasColumnName("PAirline");

                entity.Property(e => e.PaxType).HasMaxLength(10);

                entity.Property(e => e.Rairline).HasColumnName("RAirline");

                entity.Property(e => e.RestrictedClass).HasMaxLength(100);

                entity.Property(e => e.RuleName).HasMaxLength(100);

                entity.Property(e => e.SaleDateFrom).HasColumnType("date");

                entity.Property(e => e.SaleDateTo).HasColumnType("date");

                entity.Property(e => e.TravelDateFrom).HasColumnType("date");

                entity.Property(e => e.TravelDateTo).HasColumnType("date");

                entity.Property(e => e.Ttime).HasColumnName("TTime");

                entity.Property(e => e.UpdateBy).HasMaxLength(100);
            });

            modelBuilder.Entity<MasterCondition>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MasterCondition");

                entity.Property(e => e.AirlineCode).HasMaxLength(50);

                entity.Property(e => e.Destination).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Origin).HasMaxLength(50);

                entity.Property(e => e.SpecialCabinCode)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.SpecialReturnDate).HasColumnType("date");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<MasterOffersCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__master_O__23CAF1F851EF2864");

                entity.ToTable("master_OffersCategory");

                entity.Property(e => e.CategoryId).HasColumnName("categoryID");

                entity.Property(e => e.CategoryCode)
                    .HasMaxLength(100)
                    .HasColumnName("categoryCode");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .HasColumnName("categoryName");

                entity.Property(e => e.Description)
                    .HasMaxLength(4000)
                    .HasColumnName("description");

                entity.Property(e => e.ImageName).HasMaxLength(100);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsFeatured).HasColumnName("isFeatured");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<MetaSearchDestination>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MetaSearchDestination");

                entity.Property(e => e.CabinClass)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Destination)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Origin)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<MissingCity>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MissingCity");

                entity.Property(e => e.AirportCode).HasMaxLength(255);

                entity.Property(e => e.CityCode).HasMaxLength(255);

                entity.Property(e => e.CountryCode).HasMaxLength(255);
            });

            modelBuilder.Entity<NewsletterEmail>(entity =>
            {
                entity.ToTable("Newsletter_Email");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmailId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<OfferMarkupTblUpdateDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Offer_MarkupTblUpdateDetail");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.MarkupTblUpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.OfferTblUpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<OffersCategory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("offers_category");

                entity.Property(e => e.CategoryId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("category_Id");

                entity.Property(e => e.CategoryImg)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("category_img");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("category_name");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsCountrySpecial)
                    .HasColumnName("isCountrySpecial")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsFlightSpecial).HasColumnName("isFlightSpecial");

                entity.Property(e => e.IsHotelSpecial).HasColumnName("isHotelSpecial");

                entity.Property(e => e.IsHtml).HasColumnName("isHtml");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<OffersSpecial>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("offers_special");

                entity.Property(e => e.CategoryId).HasColumnName("category_Id");

                entity.Property(e => e.Destination)
                    .HasMaxLength(300)
                    .HasColumnName("destination");

                entity.Property(e => e.Fromdate)
                    .HasColumnType("date")
                    .HasColumnName("fromdate");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Offer)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("offer");

                entity.Property(e => e.Origin)
                    .HasMaxLength(300)
                    .HasColumnName("origin");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("price");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Todate)
                    .HasColumnType("date")
                    .HasColumnName("todate");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasIndex(e => new { e.WebsiteId, e.PageKey }, "UQ_Website_PageKey")
                    .IsUnique();

                entity.Property(e => e.AltImageText).HasMaxLength(255);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Delind).HasDefaultValueSql("((0))");

                entity.Property(e => e.FeatureImageUrl).HasMaxLength(500);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.PageKey)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.Property(e => e.ViewName).HasMaxLength(100);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AnetTransactionId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AuthCode).HasMaxLength(20);

                entity.Property(e => e.AvsResult).HasMaxLength(10);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Currency).HasMaxLength(10);

                entity.Property(e => e.CustomerEmail).HasMaxLength(256);

                entity.Property(e => e.CvvResult).HasMaxLength(10);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.GatewayAccount).HasMaxLength(50);

                entity.Property(e => e.GatewayEnv).HasMaxLength(20);

                entity.Property(e => e.InvoiceNumber).HasMaxLength(20);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.OrderId).HasMaxLength(100);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");
            });

            modelBuilder.Entity<PaymentEvent>(entity =>
            {
                entity.Property(e => e.EventType)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ReceivedAt).HasDefaultValueSql("(sysutcdatetime())");
            });

            modelBuilder.Entity<PaymentLink>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                entity.ToTable("paymentLink");

                entity.Property(e => e.PaymentId).HasColumnName("paymentId");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.ClientMail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("clientMail");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiryDate");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("firstName");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastName");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.Property(e => e.Subject)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("subject");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");
            });

            modelBuilder.Entity<PaymentsBlock>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("payments_block");

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ToCity)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.WebsiteId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<PermittedAirline>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PermittedAirline");

                entity.Property(e => e.AirlineCode)
                    .HasMaxLength(50)
                    .HasColumnName("airlineCode");

                entity.Property(e => e.Destination)
                    .HasMaxLength(1000)
                    .HasColumnName("destination");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Origin)
                    .HasMaxLength(1000)
                    .HasColumnName("origin");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<PermittedCountry>(entity =>
            {
                entity.ToTable("PermittedCountry");

                entity.Property(e => e.Countrycodes).HasMaxLength(4000);

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<PopularDestination>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PopularDestination");

                entity.Property(e => e.DestinationCode).HasMaxLength(10);

                entity.Property(e => e.DestinationDesc)
                    .IsUnicode(false)
                    .HasColumnName("Destination_Desc");

                entity.Property(e => e.DestinationName).HasMaxLength(30);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Keyword).HasMaxLength(200);

                entity.Property(e => e.MetaDescription).HasMaxLength(200);

                entity.Property(e => e.PageTitle).HasMaxLength(60);

                entity.Property(e => e.SystemDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PopularDestinationFlight>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PopularDestination_Flight");

                entity.Property(e => e.AirlineCode)
                    .HasMaxLength(50)
                    .HasColumnName("Airline_Code");

                entity.Property(e => e.Baggage).HasMaxLength(50);

                entity.Property(e => e.CabinClass)
                    .HasMaxLength(50)
                    .HasColumnName("Cabin_Class");

                entity.Property(e => e.ChdFare)
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Destination).HasMaxLength(50);

                entity.Property(e => e.Fare)
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.InfFare)
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.SalesTill)
                    .HasColumnType("datetime")
                    .HasColumnName("Sales_Till")
                    .HasDefaultValueSql("('1/4/2020 12:00:00 AM')");

                entity.Property(e => e.TravelFrom)
                    .HasColumnType("datetime")
                    .HasColumnName("Travel_From");

                entity.Property(e => e.TravelTo)
                    .HasColumnType("datetime")
                    .HasColumnName("Travel_To");

                entity.Property(e => e.ViaPoint).HasMaxLength(50);
            });

            modelBuilder.Entity<Quotation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Quotation");

                entity.Property(e => e.AgentId).HasColumnName("AgentID");

                entity.Property(e => e.ConsolidatorId).HasColumnName("ConsolidatorID");

                entity.Property(e => e.EmailSubject)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteAddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteAddedOn).HasColumnType("datetime");

                entity.Property(e => e.QuoteBookedFor)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteBookedForAgentCompanyId).HasColumnName("QuoteBookedForAgentCompanyID");

                entity.Property(e => e.QuoteCcemail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("QuoteCCEmail");

                entity.Property(e => e.QuoteId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("QuoteID");

                entity.Property(e => e.QuoteRefId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("QuoteRefID");

                entity.Property(e => e.QuoteRemarks)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteToEmail)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteToFirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteToLastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteToTitle)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Quotation1>(entity =>
            {
                entity.HasKey(e => e.QuotationId)
                    .HasName("PK__Quotatio__E19752937E34965C");

                entity.ToTable("Quotations");

                entity.Property(e => e.Class).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FromCity).HasMaxLength(100);

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);

                entity.Property(e => e.ToCity).HasMaxLength(100);

                entity.Property(e => e.ToDate).HasColumnType("date");
            });

            modelBuilder.Entity<QuoteExtraService>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.QuoteId).HasColumnName("QuoteID");

                entity.Property(e => e.QuoteServiceId).ValueGeneratedOnAdd();

                entity.Property(e => e.ServiceAmount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ServiceName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("serviceName");

                entity.Property(e => e.SystemDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<QuoteHotelDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QuoteHotelDetail");

                entity.Property(e => e.AdditionalEarnings).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Apiname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APIName");

                entity.Property(e => e.BookingQuoteId)
                    .IsUnicode(false)
                    .HasColumnName("BookingQuoteID");

                entity.Property(e => e.BordType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CancellationDetail).IsUnicode(false);

                entity.Property(e => e.CheckInDate).HasColumnType("date");

                entity.Property(e => e.CheckOutDate).HasColumnType("date");

                entity.Property(e => e.DestinationQuery)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FareAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.QuoteHotelAddress)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteHotelDetailId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("QuoteHotelDetailID");

                entity.Property(e => e.QuoteHotelName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteId).HasColumnName("QuoteID");

                entity.Property(e => e.QuoteOption)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoomCategory)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<QuoteTripDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QuoteTripDetail");

                entity.Property(e => e.AdditionalEarning).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AirSegmentKey)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AirlineCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AirlineLogo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AirlineName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Apiname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("APIName");

                entity.Property(e => e.ArrivalAirport)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ArrivalDateTime).HasColumnType("datetime");

                entity.Property(e => e.ArrivalDateTimeFromRsp).HasColumnType("datetime");

                entity.Property(e => e.ArrivalTerminal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BaggageAllowanceAdt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BaggageAllowanceADT");

                entity.Property(e => e.BaggageAllowanceCnn)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BaggageAllowanceCNN");

                entity.Property(e => e.BaggageAllowanceInf)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BaggageAllowanceINF");

                entity.Property(e => e.BaggageAllowanceIns)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BaggageAllowanceINS");

                entity.Property(e => e.BookDesigCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BookingClass)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BookingFareBasis)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DepartureAirport)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DepartureDateTime).HasColumnType("datetime");

                entity.Property(e => e.DepartureDateTimeFromRsp).HasColumnType("datetime");

                entity.Property(e => e.DepartureTerminal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FinalDestination)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightNum)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LayoverDetail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MarketingAirlineCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MarketingAirlineFlightNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MarketingAirlineName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MarriageGrp)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OfferIdOrResponseId).IsUnicode(false);

                entity.Property(e => e.PassengerCountDetail)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PriceBreakdownJson)
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.QuotationOrBooking)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QuoteFlightOptionId).HasColumnName("QuoteFlightOptionID");

                entity.Property(e => e.QuoteId).HasColumnName("QuoteID");

                entity.Property(e => e.QuoteTripId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("QuoteTripID");

                entity.Property(e => e.StopAirports)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StopSummary)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TotalTravelTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TripType)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RecentSearch>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Adt)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Arrival)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ArrivalDate).HasMaxLength(20);

                entity.Property(e => e.Chd)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Class)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Depart)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DepartDate).HasMaxLength(20);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Inf)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Jtype)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserHit).HasMaxLength(50);

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(10)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<SeoEntity>(entity =>
            {
                entity.HasIndex(e => new { e.WebsiteId, e.Slug }, "UQ_Website_Slug")
                    .IsUnique();

                entity.Property(e => e.CanonicalUrl).HasMaxLength(300);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Delind).HasDefaultValueSql("((0))");

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeywords).HasMaxLength(2000);

                entity.Property(e => e.MetaTitle).HasMaxLength(300);

                entity.Property(e => e.OgDescription).HasMaxLength(500);

                entity.Property(e => e.OgImageUrl).HasMaxLength(500);

                entity.Property(e => e.OgTitle).HasMaxLength(300);

                entity.Property(e => e.OgType).HasMaxLength(50);

                entity.Property(e => e.PageType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Robots).HasMaxLength(100);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Amount).HasComment("service amount");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .HasComment("service code( visa, policy etc)");

                entity.Property(e => e.Description).HasComment("service description");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))")
                    .HasComment("If service is on, It should be true");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasComment("service name");

                entity.Property(e => e.ServiceId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("serviceId");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("System Date");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .HasComment("service type (Flight/Hotel/Flight+hotel)");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .HasColumnName("websiteId")
                    .HasComment("website Id");
            });

            modelBuilder.Entity<SiteAddOn>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("site_addOn");

                entity.Property(e => e.AddOnDisplayName)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("addOnDisplayName");

                entity.Property(e => e.AddOnValue)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("addOnValue");

                entity.Property(e => e.ChargeType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("chargeType")
                    .IsFixedLength(true);

                entity.Property(e => e.Charges)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("charges");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Websiteid)
                    .HasMaxLength(50)
                    .HasColumnName("websiteid");
            });

            modelBuilder.Entity<SiteAddonmaster>(entity =>
            {
                entity.ToTable("site_addonmaster");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddOnValue)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("addOnValue");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Name)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<SiteId>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SiteID");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .HasColumnName("WebsiteID");

                entity.Property(e => e.WebsiteName).HasMaxLength(50);
            });

            modelBuilder.Entity<SiteMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SiteMaster");

                entity.HasComment("delete skyres_AgentId,skyres_AgentPwd,skyres_AccountNo,BookedOffers,certPath,DefaultMarkup,saBRID,saID,WebSiteID");

                entity.Property(e => e.AdminEmailId)
                    .HasMaxLength(100)
                    .HasColumnName("Admin_EmailId");

                entity.Property(e => e.AllowedHotelRatings)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("allowed_hotelRatings");

                entity.Property(e => e.BookedOffers).HasMaxLength(50);

                entity.Property(e => e.BookingMethodType)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.CertPath)
                    .HasMaxLength(100)
                    .HasColumnName("certPath");

                entity.Property(e => e.CompanyName).HasMaxLength(100);

                entity.Property(e => e.ContactNo).HasMaxLength(20);

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.CurrencySymbol)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("currencySymbol")
                    .IsFixedLength(true);

                entity.Property(e => e.DefaultMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.GdsUrl)
                    .HasMaxLength(100)
                    .HasColumnName("gdsUrl");

                entity.Property(e => e.IsFlightPlusHotel).HasColumnName("isFlightPlusHotel");

                entity.Property(e => e.IsLivePaymentGateway).HasColumnName("isLivePaymentGateway");

                entity.Property(e => e.IsPackage).HasColumnName("isPackage");

                entity.Property(e => e.IsRounded).HasColumnName("isRounded");

                entity.Property(e => e.IsTransparent).HasColumnName("isTransparent");

                entity.Property(e => e.Language)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.MasterTemplate)
                    .HasMaxLength(3)
                    .HasColumnName("masterTemplate")
                    .IsFixedLength(true);

                entity.Property(e => e.PaymentGateway)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("paymentGateway");

                entity.Property(e => e.PrefferedSearchEngine).HasMaxLength(50);

                entity.Property(e => e.SaAccountNo).HasMaxLength(50);

                entity.Property(e => e.SaBrid)
                    .HasMaxLength(50)
                    .HasColumnName("saBRID");

                entity.Property(e => e.SaId)
                    .HasMaxLength(50)
                    .HasColumnName("saID");

                entity.Property(e => e.SeAppDomain).HasMaxLength(1000);

                entity.Property(e => e.SePaymentHandlerGateway)
                    .HasMaxLength(1000)
                    .HasColumnName("sePaymentHandlerGateway");

                entity.Property(e => e.SearchEngineBg)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SearchEngineBG")
                    .HasDefaultValueSql("('black.css')");

                entity.Property(e => e.SearchType).HasMaxLength(50);

                entity.Property(e => e.SiteId).ValueGeneratedOnAdd();

                entity.Property(e => e.SiteReference)
                    .HasMaxLength(50)
                    .HasColumnName("siteReference");

                entity.Property(e => e.SiteUrl).HasMaxLength(100);

                entity.Property(e => e.SkyresAccountNo)
                    .HasMaxLength(50)
                    .HasColumnName("skyres_AccountNo");

                entity.Property(e => e.SkyresAgentId)
                    .HasMaxLength(100)
                    .HasColumnName("skyres_AgentId");

                entity.Property(e => e.SkyresAgentPwd)
                    .HasMaxLength(50)
                    .HasColumnName("skyres_AgentPwd");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TargetBranch)
                    .HasMaxLength(10)
                    .HasColumnName("targetBranch")
                    .IsFixedLength(true);

                entity.Property(e => e.Template)
                    .HasMaxLength(3)
                    .HasColumnName("template")
                    .IsFixedLength(true);

                entity.Property(e => e.TemplateCustom)
                    .HasMaxLength(50)
                    .HasColumnName("templateCustom")
                    .IsFixedLength(true);

                entity.Property(e => e.TermUrl)
                    .HasMaxLength(200)
                    .HasColumnName("termUrl");

                entity.Property(e => e.ThemeColor)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WebSiteId)
                    .HasMaxLength(50)
                    .HasColumnName("WebSiteID");
            });

            modelBuilder.Entity<SiteMiscService>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("site_miscService");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ServiceId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("serviceId");

                entity.Property(e => e.ServiceName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("serviceName");

                entity.Property(e => e.ServiceValue)
                    .IsUnicode(false)
                    .HasColumnName("serviceValue");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<SiteSeo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("site_seo");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.MetaDescription)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("metaDescription");

                entity.Property(e => e.MetaKeyword)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("metaKeyword");

                entity.Property(e => e.PageName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pageName");

                entity.Property(e => e.PageTitle)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pageTitle");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<SiteServiceMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("site_serviceMaster");

                entity.Property(e => e.ServiceMasterId).HasColumnName("serviceMasterId");

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("serviceName");

                entity.Property(e => e.ServiceText)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("serviceText");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<SpecialJourney>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SpecialJourney");

                entity.Property(e => e.AirlineCode).HasMaxLength(3000);

                entity.Property(e => e.Destination).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.InboundCabinCode)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Origin).HasMaxLength(50);

                entity.Property(e => e.OutboundCabinCode)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<SpecialOffer>(entity =>
            {
                entity.ToTable("SpecialOffer");

                entity.Property(e => e.Airline)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Amount_");

                entity.Property(e => e.CabinClass)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CityCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CountryName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateFrom)
                    .HasColumnType("date")
                    .HasColumnName("Date_From");

                entity.Property(e => e.DateTo)
                    .HasColumnType("date")
                    .HasColumnName("Date_To");

                entity.Property(e => e.DepositPercent)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Deposit_Percent");

                entity.Property(e => e.Destination)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ExclusionId).HasMaxLength(100);

                entity.Property(e => e.FareType)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.GeneralOfferUrl)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("GeneralOfferURL");

                entity.Property(e => e.HotelName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Images)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("Images_");

                entity.Property(e => e.InclusionExclusionId).HasMaxLength(100);

                entity.Property(e => e.IsAutomated).HasColumnName("isAutomated");

                entity.Property(e => e.JournyType)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MetaDescrption)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.MetaKeywords)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OfferType)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.OffersCategory).HasMaxLength(100);

                entity.Property(e => e.Origin)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Title).HasMaxLength(1000);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<SpecialOfferCategory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SpecialOffer_Category");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Category_Name");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SystemDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SpecialOfferItnaryDetail>(entity =>
            {
                entity.ToTable("SpecialOffer_ItnaryDetails");

                entity.Property(e => e.Title)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SpecialOfferMiscellaneou>(entity =>
            {
                entity.ToTable("SpecialOffer_Miscellaneous");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Heading).HasMaxLength(1000);

                entity.Property(e => e.SpecialOfferId).HasColumnName("SpecialOffer_Id");

                entity.HasOne(d => d.SpecialOffer)
                    .WithMany(p => p.SpecialOfferMiscellaneous)
                    .HasForeignKey(d => d.SpecialOfferId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_specialOffers_Miscellaneous_specialOffer_Id");
            });

            modelBuilder.Entity<StCity>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("st_City");

                entity.Property(e => e.CityCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.ProvinceCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ProvinceCountryCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<StCityName>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("st_CityName");

                entity.Property(e => e.AltCityName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CityCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LanguageCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Prioritycode)
                    .HasMaxLength(1)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<StCountry>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("st_Country");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DefaultLanguageCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.GeogLoc)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("Geog_loc");

                entity.Property(e => e.IsdCode)
                    .HasMaxLength(255)
                    .HasColumnName("ISD_Code");

                entity.Property(e => e.Nationality)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nationality");

                entity.Property(e => e.RelatedCurrencyCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ZoneCode).HasColumnName("zone_code");
            });

            modelBuilder.Entity<StCountryName>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("st_CountryName");

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CountryName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Images).HasMaxLength(100);

                entity.Property(e => e.IsFeatured).HasColumnName("isFeatured");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Nationality)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<StartRatingSelected>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("StartRating_Selected");

                entity.Property(e => e.Destination)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Taxinfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("taxinfo");

                entity.Property(e => e.AdtYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("adtYQ");

                entity.Property(e => e.Adttax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("adttax");

                entity.Property(e => e.Airline)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("airline")
                    .IsFixedLength(true);

                entity.Property(e => e.Arrive)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("arrive")
                    .IsFixedLength(true);

                entity.Property(e => e.Cabin)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cabin")
                    .IsFixedLength(true);

                entity.Property(e => e.ChdYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("chdYQ");

                entity.Property(e => e.Chdtax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("chdtax");

                entity.Property(e => e.Class)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("class")
                    .IsFixedLength(true);

                entity.Property(e => e.CrDt)
                    .HasColumnType("datetime")
                    .HasColumnName("cr_dt");

                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("currency")
                    .IsFixedLength(true);

                entity.Property(e => e.Depart)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("depart")
                    .IsFixedLength(true);

                entity.Property(e => e.DepartDate).HasColumnType("datetime");

                entity.Property(e => e.Ftype)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ftype")
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("id")
                    .IsFixedLength(true);

                entity.Property(e => e.InfYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("infYQ");

                entity.Property(e => e.Inftax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("inftax");

                entity.Property(e => e.LastDt)
                    .HasColumnType("datetime")
                    .HasColumnName("last_dt");

                entity.Property(e => e.NdAdtYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_adtYQ");

                entity.Property(e => e.NdAdtYq3)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_adtYQ_3");

                entity.Property(e => e.NdAdttax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_adttax");

                entity.Property(e => e.NdAdttax3)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_adttax_3");

                entity.Property(e => e.NdChdYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_chdYQ");

                entity.Property(e => e.NdChdYq3)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_chdYQ_3");

                entity.Property(e => e.NdChdtax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_chdtax");

                entity.Property(e => e.NdChdtax3)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_chdtax_3");

                entity.Property(e => e.NdInfYq)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_infYQ");

                entity.Property(e => e.NdInfYq3)
                    .HasColumnType("smallmoney")
                    .HasColumnName("nd_infYQ_3");

                entity.Property(e => e.NdInftax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_inftax");

                entity.Property(e => e.NdInftax3)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_inftax_3");

                entity.Property(e => e.NdYthTax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_ythTax");

                entity.Property(e => e.NdYthTax3)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("nd_ythTax_3");

                entity.Property(e => e.ReturnDate).HasColumnType("datetime");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");

                entity.Property(e => e.YthTax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ythTax");
            });

            modelBuilder.Entity<TblCampaign>(entity =>
            {
                entity.ToTable("tblCampaigns");

                entity.Property(e => e.CampId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("CampID");

                entity.Property(e => e.Company).HasMaxLength(200);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExtraMarkup).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<TblHorizontalBanner>(entity =>
            {
                entity.ToTable("tblHorizontalBanners");

                entity.Property(e => e.BannerImage)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Link).IsUnicode(false);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblmanageIp>(entity =>
            {
                entity.ToTable("tblmanageIP");

                entity.Property(e => e.Ipaddress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("IPAddress");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteID");
            });

            modelBuilder.Entity<TempSiteMasterzz>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tempSiteMasterzz");

                entity.Property(e => e.AdminEmailId)
                    .HasMaxLength(100)
                    .HasColumnName("Admin_EmailId");

                entity.Property(e => e.AllowedHotelRatings)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("allowed_hotelRatings");

                entity.Property(e => e.BookedOffers).HasMaxLength(50);

                entity.Property(e => e.CertPath)
                    .HasMaxLength(100)
                    .HasColumnName("certPath");

                entity.Property(e => e.CompanyName).HasMaxLength(100);

                entity.Property(e => e.ContactNo).HasMaxLength(20);

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.CurrencySymbol)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("currencySymbol")
                    .IsFixedLength(true);

                entity.Property(e => e.DefaultMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.GdsUrl)
                    .HasMaxLength(100)
                    .HasColumnName("gdsUrl");

                entity.Property(e => e.IsFlightPlusHotel).HasColumnName("isFlightPlusHotel");

                entity.Property(e => e.IsLivePaymentGateway).HasColumnName("isLivePaymentGateway");

                entity.Property(e => e.IsPackage).HasColumnName("isPackage");

                entity.Property(e => e.IsRounded).HasColumnName("isRounded");

                entity.Property(e => e.IsTransparent).HasColumnName("isTransparent");

                entity.Property(e => e.Language)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.MasterTemplate)
                    .HasMaxLength(3)
                    .HasColumnName("masterTemplate")
                    .IsFixedLength(true);

                entity.Property(e => e.PaymentGateway)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("paymentGateway");

                entity.Property(e => e.PrefferedSearchEngine).HasMaxLength(50);

                entity.Property(e => e.SaAccountNo).HasMaxLength(50);

                entity.Property(e => e.SaBrid)
                    .HasMaxLength(50)
                    .HasColumnName("saBRID");

                entity.Property(e => e.SaId)
                    .HasMaxLength(50)
                    .HasColumnName("saID");

                entity.Property(e => e.SeAppDomain).HasMaxLength(1000);

                entity.Property(e => e.SePaymentHandlerGateway)
                    .HasMaxLength(1000)
                    .HasColumnName("sePaymentHandlerGateway");

                entity.Property(e => e.SearchEngineBg)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SearchEngineBG");

                entity.Property(e => e.SearchType).HasMaxLength(50);

                entity.Property(e => e.SiteId).ValueGeneratedOnAdd();

                entity.Property(e => e.SiteReference)
                    .HasMaxLength(50)
                    .HasColumnName("siteReference");

                entity.Property(e => e.SiteUrl).HasMaxLength(100);

                entity.Property(e => e.SkyresAccountNo)
                    .HasMaxLength(50)
                    .HasColumnName("skyres_AccountNo");

                entity.Property(e => e.SkyresAgentId)
                    .HasMaxLength(100)
                    .HasColumnName("skyres_AgentId");

                entity.Property(e => e.SkyresAgentPwd)
                    .HasMaxLength(50)
                    .HasColumnName("skyres_AgentPwd");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.TargetBranch)
                    .HasMaxLength(10)
                    .HasColumnName("targetBranch")
                    .IsFixedLength(true);

                entity.Property(e => e.Template)
                    .HasMaxLength(3)
                    .HasColumnName("template")
                    .IsFixedLength(true);

                entity.Property(e => e.TemplateCustom)
                    .HasMaxLength(50)
                    .HasColumnName("templateCustom")
                    .IsFixedLength(true);

                entity.Property(e => e.TermUrl)
                    .HasMaxLength(200)
                    .HasColumnName("termUrl");

                entity.Property(e => e.ThemeColor)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WebSiteId)
                    .HasMaxLength(50)
                    .HasColumnName("WebSiteID");
            });

            modelBuilder.Entity<TemplateMaster>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("templateMaster");

                entity.Property(e => e.AdminDisplayName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("adminDisplayName");

                entity.Property(e => e.Category)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("category");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.PageName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pageName");
            });

            modelBuilder.Entity<Testimonial>(entity =>
            {
                entity.ToTable("testimonials");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Designation)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("designation");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Rating)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("rating");

                entity.Property(e => e.UserImage)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("userImage");

                entity.Property(e => e.WebSiteId)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TheameColor>(entity =>
            {
                entity.ToTable("TheameColor");

                entity.Property(e => e.Color)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UmrahOfferDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UmrahOfferDetail");

                entity.Property(e => e.Destination).HasMaxLength(50);

                entity.Property(e => e.Duration).HasMaxLength(50);

                entity.Property(e => e.Heading).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Image).HasMaxLength(50);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("user_login");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("firstName");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LastLoginDate)
                    .HasColumnType("datetime")
                    .HasColumnName("lastLogin_date");

                entity.Property(e => e.LastLoginIpAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastLogin_IpAddress");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lastName");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<UserSearch>(entity =>
            {
                entity.ToTable("user_search");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Destination)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("destination");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Origin)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SavedUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("savedUrl");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.TravelDateFrom)
                    .HasColumnType("date")
                    .HasColumnName("travelDateFrom");

                entity.Property(e => e.TravelDateTo)
                    .HasColumnType("date")
                    .HasColumnName("travelDateTo");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserType");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.UserCode)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<Userlog>(entity =>
            {
                entity.ToTable("Userlog");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Message).HasMaxLength(255);

                entity.Property(e => e.SystemDate).HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.HasIndex(e => e.Domain, "UQ__Websites__FD349E53148BDC20")
                    .IsUnique();

                entity.Property(e => e.Domain).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<WebsiteContact>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("websiteContacts");

                entity.Property(e => e.CabinClass)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cabinClass")
                    .IsFixedLength(true);

                entity.Property(e => e.ContactNo)
                    .HasMaxLength(20)
                    .HasColumnName("contactNo");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .HasColumnName("websiteId");
            });

            modelBuilder.Entity<WlsCustomBusinessOffer>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("wls_Custom_BusinessOffers");

                entity.Property(e => e.AirlineCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CabinClassCode)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DepartureFrom)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Destination)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Fare).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.SaAccountNo).HasMaxLength(50);

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<WlsCustomFooter>(entity =>
            {
                entity.ToTable("wls_CustomFooter");

                entity.Property(e => e.FbLink).HasMaxLength(50);

                entity.Property(e => e.GPlusLink)
                    .HasMaxLength(50)
                    .HasColumnName("gPlusLink");

                entity.Property(e => e.InstaLink).HasMaxLength(50);

                entity.Property(e => e.SaAccountNo).HasMaxLength(50);

                entity.Property(e => e.TwitterLink).HasMaxLength(50);

                entity.Property(e => e.WebsiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("websiteId");

                entity.Property(e => e.YoutubeLink)
                    .HasMaxLength(50)
                    .HasColumnName("youtubeLink");
            });

            modelBuilder.Entity<WlsCustomHeader>(entity =>
            {
                entity.ToTable("wls_CustomHeader");

                entity.Property(e => e.SaAccount).HasMaxLength(50);

                entity.Property(e => e.Timing).HasMaxLength(200);
            });

            modelBuilder.Entity<Zztemp>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("zztemp");

                entity.Property(e => e.AdminEmailId)
                    .HasMaxLength(100)
                    .HasColumnName("Admin_EmailId");

                entity.Property(e => e.AllowedHotelRatings)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("allowed_hotelRatings");

                entity.Property(e => e.BookedOffers).HasMaxLength(50);

                entity.Property(e => e.CertPath)
                    .HasMaxLength(100)
                    .HasColumnName("certPath");

                entity.Property(e => e.CompanyName).HasMaxLength(100);

                entity.Property(e => e.ContactNo).HasMaxLength(20);

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.CurrencySymbol)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("currencySymbol")
                    .IsFixedLength(true);

                entity.Property(e => e.DefaultMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.GdsUrl)
                    .HasMaxLength(100)
                    .HasColumnName("gdsUrl");

                entity.Property(e => e.IsFlightPlusHotel).HasColumnName("isFlightPlusHotel");

                entity.Property(e => e.IsLivePaymentGateway).HasColumnName("isLivePaymentGateway");

                entity.Property(e => e.IsPackage).HasColumnName("isPackage");

                entity.Property(e => e.IsRounded).HasColumnName("isRounded");

                entity.Property(e => e.IsTransparent).HasColumnName("isTransparent");

                entity.Property(e => e.Language)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.MasterTemplate)
                    .HasMaxLength(3)
                    .HasColumnName("masterTemplate")
                    .IsFixedLength(true);

                entity.Property(e => e.PaymentGateway)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("paymentGateway");

                entity.Property(e => e.PrefferedSearchEngine).HasMaxLength(50);

                entity.Property(e => e.SaAccountNo).HasMaxLength(50);

                entity.Property(e => e.SaBrid)
                    .HasMaxLength(50)
                    .HasColumnName("saBRID");

                entity.Property(e => e.SaId)
                    .HasMaxLength(50)
                    .HasColumnName("saID");

                entity.Property(e => e.SeAppDomain).HasMaxLength(1000);

                entity.Property(e => e.SePaymentHandlerGateway)
                    .HasMaxLength(1000)
                    .HasColumnName("sePaymentHandlerGateway");

                entity.Property(e => e.SearchEngineBg)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SearchEngineBG");

                entity.Property(e => e.SearchType).HasMaxLength(50);

                entity.Property(e => e.SiteId).ValueGeneratedOnAdd();

                entity.Property(e => e.SiteReference)
                    .HasMaxLength(50)
                    .HasColumnName("siteReference");

                entity.Property(e => e.SiteUrl).HasMaxLength(100);

                entity.Property(e => e.SkyresAccountNo)
                    .HasMaxLength(50)
                    .HasColumnName("skyres_AccountNo");

                entity.Property(e => e.SkyresAgentId)
                    .HasMaxLength(100)
                    .HasColumnName("skyres_AgentId");

                entity.Property(e => e.SkyresAgentPwd)
                    .HasMaxLength(50)
                    .HasColumnName("skyres_AgentPwd");

                entity.Property(e => e.SystemDate)
                    .HasColumnType("datetime")
                    .HasColumnName("systemDate");

                entity.Property(e => e.TargetBranch)
                    .HasMaxLength(10)
                    .HasColumnName("targetBranch")
                    .IsFixedLength(true);

                entity.Property(e => e.Template)
                    .HasMaxLength(3)
                    .HasColumnName("template")
                    .IsFixedLength(true);

                entity.Property(e => e.TemplateCustom)
                    .HasMaxLength(50)
                    .HasColumnName("templateCustom")
                    .IsFixedLength(true);

                entity.Property(e => e.TermUrl)
                    .HasMaxLength(200)
                    .HasColumnName("termUrl");

                entity.Property(e => e.ThemeColor)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WebSiteId)
                    .HasMaxLength(50)
                    .HasColumnName("WebSiteID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
