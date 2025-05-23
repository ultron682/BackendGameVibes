﻿using BackendGameVibes.Models.Forum;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.DTOs.Forum {
    public class NewForumThreadDTO {
        [Required]
        public string? Title {
            get; set;
        }
        [Required]
        public int? SectionId {
            get; set;
        }
        [Required]
        public string? FirstForumPostContent {
            get; set;
        }
    }
}
