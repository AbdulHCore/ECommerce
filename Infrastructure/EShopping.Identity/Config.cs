// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace EShopping.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalogapi"), //API Level full access.
                new ApiScope("basketapi"),
                new ApiScope("catalogapi.read"), //API Level specific access (Read)
                new ApiScope("catalogapi.write"), //API Level specific access (write)
                new ApiScope("eshoppinggateway") //Gateway level Scope
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("Catalog", "Catalog.API")
                {
                    Scopes = {"catalogapi.read","catalogapi.write"} //Specific scope level access enalbed.
                },
                new ApiResource("Basket", "Basket.API")
                {
                    Scopes = { "basketapi" }
                },
                new ApiResource("EShoppingGateway", "EShopping Gateway")
                {
                      Scopes = { "eshoppinggateway" }
                },
                new ApiResource("Ordering", "Ordering.API")
                {
                    Scopes = { "orderingapi" }
                },
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
              //Machine to Machine flow
              new Client
              {
                  ClientName = "Catalog API Client",
                  ClientId = "CatalogApiClient",
                  ClientSecrets = { new Secret ("0ed6bb47-bfd3-4e50-b2f2-0c762d12c3ff".Sha256())},
                  AllowedGrantTypes = GrantTypes.ClientCredentials,
                  AllowedScopes = { "catalogapi.read", "catalogapi.write" }
              },
              new Client
              {
                  ClientName = "Basket API Client",
                  ClientId = "BasketApiClient",
                  ClientSecrets = { new Secret ("0ed6c446-bfd3-4e50-b2f2-0c762d12c3ff".Sha256())},
                  AllowedGrantTypes = GrantTypes.ClientCredentials,
                  AllowedScopes = {"basketapi" }
              },
              new Client
              {
                  ClientName = "Ordering API Client",
                  ClientId = "OrderApiClient",
                  ClientSecrets = { new Secret ("0ed6c777-bfd3-4e50-b2f2-0c762d12c3ff".Sha256())},
                  AllowedGrantTypes = GrantTypes.ClientCredentials,
                  AllowedScopes = {"orderingapi" }
              },
              new Client
              {
                  ClientName = "EShopping Gateway Client",
                  ClientId ="EshoppingGatewayClient",
                  ClientSecrets = { new Secret("52ade23b-a315-495b-a9e0-f7a0de316cae".Sha256())},
                  AllowedGrantTypes = GrantTypes.ClientCredentials,
                  AllowedScopes = { "eshoppinggateway", "basketapi" }
              }
            };
    }
}