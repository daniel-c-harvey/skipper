```mermaid
erDiagram

    CUSTOMER {
        long Id PK
        string AccountNumber
        string Name
        string CustomerProfileType "CustomerProfileType enum"
        long CustomerProfileId FK
    }

    VESSEL_OWNER_PROFILE {
        long Id PK
        long ContactId FK
    }

    INDIVIDUAL_CUSTOMER_PROFILE {
        long Id PK
        long ContactId FK
    }

    BUSINESS_CUSTOMER_PROFILE {
        long Id PK
        string BusinessName
        string TaxId
        long PrimaryContactId FK
    }

    BUSINESS_CUSTOMER_CONTACT {
        long Id PK
        long BusinessCustomerProfileId FK
        long ContactId FK
        bool IsPrimary
        bool IsEmergency
    }

    MEMBER_CUSTOMER_PROFILE {
        long Id PK
        long ContactId FK
        datetime MembershipStartDate
        datetime MembershipEndDate
        string MembershipLevel
    }

    VESSEL_OWNER_VESSEL {
        long Id PK
        long VesselOwnerProfileId FK
        long VesselId FK
    }

    CONTACT {
        long Id PK
        string FirstName
        string LastName
        string Email
        string PhoneNumber
        long AddressId FK
    }

    ADDRESS {
        string Address1
        string Address2
        string City
        string State
        string ZipCode
        string Country
    }

    ORDER {
        long Id PK
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
        string OrderNumber
        long CustomerId FK
        datetime OrderDate
        string OrderType "OrderType enum"
        long OrderTypeId FK
        decimal TotalAmount
        string Notes
        string Status "OrderStatus enum"
    }

    CUSTOMER ||--|| VESSEL_OWNER_PROFILE : "has profile"
    CUSTOMER ||--|| INDIVIDUAL_CUSTOMER_PROFILE : "has profile"
    CUSTOMER ||--|| BUSINESS_CUSTOMER_PROFILE : "has profile"
    CUSTOMER ||--|| MEMBER_CUSTOMER_PROFILE : "has profile"
    VESSEL_OWNER_PROFILE ||--|| CONTACT : has
    INDIVIDUAL_CUSTOMER_PROFILE ||--|| CONTACT : has
    BUSINESS_CUSTOMER_PROFILE ||--|| CONTACT : "primary contact"
    BUSINESS_CUSTOMER_PROFILE ||--|{ BUSINESS_CUSTOMER_CONTACT : "additional contacts"
    BUSINESS_CUSTOMER_CONTACT ||--|| CONTACT : has
    MEMBER_CUSTOMER_PROFILE ||--|| CONTACT : has
    VESSEL_OWNER_PROFILE ||--|{ VESSEL_OWNER_VESSEL : relates
    VESSEL_OWNER_VESSEL ||--|| VESSEL : relates
    CONTACT ||--|| ADDRESS : has

    SLIP_CLASSIFICATION {
        long Id PK
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
        string Name
        decimal MaxLength
        decimal MaxBeam
        int BasePrice
        string Description
    }
    
    SLIP {
        long Id PK
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
        long SlipClassificationId FK
        string SlipNumber
        string LocationCode
        string Status
    }
    
    VESSEL {
        long Id PK
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
        string RegistrationNumber
        string Name
        decimal Length
        decimal Beam
        string VesselType
    }
    
    RENTAL_AGREEMENT {
        long Id PK
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted
        long SlipId FK
        long VesselId FK
        datetime StartDate
        datetime EndDate
        int PriceRate
        string PriceUnit
        string Status
    }
    
    SLIP_CLASSIFICATION ||--o{ SLIP : "classifies"
    SLIP ||--o{ RENTAL_AGREEMENT : "rented_through"
    VESSEL ||--o{ RENTAL_AGREEMENT : "rented_through"
    CUSTOMER ||--o{ ORDER : "places"
    ORDER ||--|| RENTAL_AGREEMENT : "polymorphic_link"