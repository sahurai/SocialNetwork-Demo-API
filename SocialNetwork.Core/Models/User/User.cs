﻿using System;
using System.Collections.Generic;
using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models
{
    /// <summary>
    /// Represents a user in the social network.
    /// </summary>
    public class User : BaseClass
    {
        public string Username { get; }
        public string Email { get; }
        public string PasswordHash { get; }
        public ICollection<Guid> Posts { get; } = new List<Guid>();
        public ICollection<Guid> Friendships { get; } = new List<Guid>();
        public ICollection<Guid> Groups { get; } = new List<Guid>();
        public ICollection<Guid> BlockedUsers { get; } = new List<Guid>();

        // Private constructor
        private User(Guid id, string username, string email, string passwordHash)
        {
            Id = id;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
        }

        // Static method to create a new User with validation
        public static (User? User, string Error) Create(
            string userName,
            string email,
            string passwordHash)
        {
            var user = new User(Guid.NewGuid(), userName, email, passwordHash);

            // Validate the instance
            var validator = new UserValidator();
            var validationResult = validator.Validate(user);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors);
                return (null, error);
            }

            return (user, string.Empty);
        }

        // Method to recreate an instance from database data
        public static (User? User, string Error) CreateFromDb(
            Guid id,
            string userName,
            string email,
            string passwordHash,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var user = new User(id, userName, email, passwordHash)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (user, string.Empty);
        }
    }
}