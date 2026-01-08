using AutoMapper;
using AutoMapper.Configuration.Conventions;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using log4net;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.Json;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;
using TrackForUBB.Service.FileHeaderMapper;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

namespace TrackForUBB.Service;

public class AcademicsService(IAcademicRepository academicRepository, IUserRepository userRepository, 
    IValidatorFactory validatorFactory, IEmailProvider emailProvider, IMapper mapper) : IAcademicsService
{
    private readonly IAcademicRepository _academicRepository = academicRepository;
    private readonly IUserRepository _userRepository = userRepository;

    private readonly ILog _logger = LogManager.GetLogger(typeof(AcademicsService));
    private readonly IValidatorFactory _validatorFactory = validatorFactory;
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly IMapper _mapper = mapper;

    public async Task<FacultyResponseDTO> CreateFaculty(FacultyPostDTO facultyPostDto)
    {
        _logger.InfoFormat("Validating FacultyPostDTO: {0}", JsonSerializer.Serialize(facultyPostDto));
        var validator = _validatorFactory.Get<FacultyPostDTO>();
        var validationResult = await validator.ValidateAsync(facultyPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new faculty to repository: {0}", JsonSerializer.Serialize(facultyPostDto));
        var facultyDto = await _academicRepository.AddFacultyAsync(facultyPostDto);

        return facultyDto;
    }

    public async Task<PromotionResponseDTO> CreatePromotion(PromotionPostDTO promotionPostDTO)
    {
        _logger.InfoFormat("Validating PromotionPostDTO: {0}", JsonSerializer.Serialize(promotionPostDTO));
        var validator = _validatorFactory.Get<PromotionPostDTO>();
        var validationResult = await validator.ValidateAsync(promotionPostDTO);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new group year to repository: {0}", JsonSerializer.Serialize(promotionPostDTO));
        var groupYearDto = await _academicRepository.AddPromotionAsync(promotionPostDTO);

        return groupYearDto;
    }

    public async Task<SpecialisationResponseDTO> CreateSpecialisation(SpecialisationPostDTO specialisationPostDto)
    {
        _logger.InfoFormat("Validating SpecialisationPostDTO: {0}", JsonSerializer.Serialize(specialisationPostDto));
        var validator = _validatorFactory.Get<SpecialisationPostDTO>();
        var validationResult = await validator.ValidateAsync(specialisationPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new specialisation to repository: {0}", JsonSerializer.Serialize(specialisationPostDto));
        var specialisationDto = await _academicRepository.AddSpecialisationAsync(specialisationPostDto);

        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> CreateStudentGroup(StudentGroupPostDTO studentGroupPostDto)
    {
        _logger.InfoFormat("Validating StudentGroupPostDTO: {0}", JsonSerializer.Serialize(studentGroupPostDto));
        var validator = _validatorFactory.Get<StudentGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new student group to repository: {0}", JsonSerializer.Serialize(studentGroupPostDto));
		var studentGroupDto = await _academicRepository.AddGroupAsync(studentGroupPostDto);

        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> CreateStudentSubGroup(StudentSubGroupPostDTO studentSubGroupPostDto)
    {
        _logger.InfoFormat("Validating StudentSubGroupPostDTO: {0}", JsonSerializer.Serialize(studentSubGroupPostDto));
        var validator = _validatorFactory.Get<StudentSubGroupPostDTO>();
        var validationResult = await validator.ValidateAsync(studentSubGroupPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new student sub-group to repository: {0}", JsonSerializer.Serialize(studentSubGroupPostDto));
		var studentSubGroupDto = await _academicRepository.AddSubGroupAsync(studentSubGroupPostDto);

        return studentSubGroupDto;
    }

    public async Task<EnrollmentResponseDTO> CreateUserEnrollment(EnrollmentPostDTO enrollmentPostDto)
    {
        _logger.InfoFormat("Validating EnrollmentPostDTO: {0}", JsonSerializer.Serialize(enrollmentPostDto));
        var validator = _validatorFactory.Get<EnrollmentPostDTO>();
        var validationResult = await validator.ValidateAsync(enrollmentPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new enrollment to repository: {0}", JsonSerializer.Serialize(enrollmentPostDto));
        var enrollmentDto = await _academicRepository.AddEnrollmentAsync(enrollmentPostDto);

        _logger.InfoFormat($"Sending email to: {enrollmentDto.User.Email}");
        await SendAddedEnrollementEmail(enrollmentDto);

        _logger.InfoFormat("Mapping enrollment entity to DTO for user with ID {0}", enrollmentPostDto.UserId);

        return enrollmentDto;
    }

    private async Task SendAddedEnrollementEmail(EnrollmentResponseDTO enrollment)
    {
        var enrollmentModel = new CreatedEnrollmentModel
        {
            UserFirstName = enrollment.User.FirstName,
            UserLastName = enrollment.User.LastName,
            GroupName = enrollment.SubGroup.Name
        };

        await _emailProvider.SendCreateEnrollmentEmailAsync(enrollment.User.Email, enrollmentModel);
    }

    public async Task<FacultyResponseDTO> GetFacultyById(int facultyId)
    {
        _logger.InfoFormat("Trying to retrieve faculty with id {0}", facultyId);
        var facultyDto = await _academicRepository.GetFacultyByIdAsync(facultyId)
            ?? throw new NotFoundException($"Faculty with ID {facultyId} not found.");

        _logger.InfoFormat("Mapping faculty entity to DTO for ID {0}", facultyId);

        return facultyDto;
    }

    public async Task<PromotionResponseDTO> GetPromotionById(int promotionId)
    {
        _logger.InfoFormat("Trying to retrieve promotion with id {0}", promotionId);
        var promotionDto = await _academicRepository.GetPromotionByIdAsync(promotionId)
            ?? throw new NotFoundException($"Promotion with ID {promotionId} not found.");

        _logger.InfoFormat("Mapped promotion entity to DTO {0}", JsonSerializer.Serialize(promotionDto));

        return promotionDto;
    }

    public async Task<SpecialisationResponseDTO> GetSpecialisationById(int specialisationId)
    {
        _logger.InfoFormat("Trying to retrieve specialisation with id {0}", specialisationId);
        var specialisationDto = await _academicRepository.GetSpecialisationByIdAsync(specialisationId)
            ?? throw new NotFoundException($"Specialisation with ID {specialisationId} not found.");

        _logger.InfoFormat("Mapping specialisation entity to DTO for ID {0}", specialisationId);

        return specialisationDto;
    }

    public async Task<StudentGroupResponseDTO> GetStudentGroupById(int studentGroupId)
    {
        _logger.InfoFormat("Trying to retrieve student group with id {0}", studentGroupId);
        var studentGroupDto = await _academicRepository.GetGroupByIdAsync(studentGroupId)
            ?? throw new NotFoundException($"StudentGroup with ID {studentGroupId} not found.");

        _logger.InfoFormat("Mapping student group entity to DTO for ID {0}", studentGroupId);

        return studentGroupDto;
    }

    public async Task<StudentSubGroupResponseDTO> GetStudentSubGroupById(int studentSubGroupId)
    {
        _logger.InfoFormat("Trying to retrieve student sub-group with id {0}", studentSubGroupId);
        var studentSubGroupDto = await _academicRepository.GetSubGroupByIdAsync(studentSubGroupId)
           ?? throw new NotFoundException($"StudentSubGroup with ID {studentSubGroupId} not found.");

        _logger.InfoFormat("Mapping student sub-group entity to DTO for ID {0}", studentSubGroupId);

        return studentSubGroupDto;
    }

    public async Task<List<EnrollmentResponseDTO>> GetUserEnrollments(int userId)
    {
        _logger.InfoFormat("Trying to retrieve enrollment for user with ID {0}", userId);

        var _ = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"Student with ID {userId} not found.");

        var enrollmentsDto = await _academicRepository.GetEnrollmentsByUserId(userId)
            ?? throw new NotFoundException($"Enrollment for user with ID {userId} not found.");

        _logger.InfoFormat("Mapping enrollment entity to DTO for user with ID {0}", userId);

        return enrollmentsDto;
    }

    public async Task<TeacherResponseDTO> CreateTeacher(TeacherPostDTO teacherPostDTO)
    {
        _logger.InfoFormat("Validating TeacherPostDTO: {0}", JsonSerializer.Serialize(teacherPostDTO));

        var validator = _validatorFactory.Get<TeacherPostDTO>();
        var validationResult = await validator.ValidateAsync(teacherPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
        }

        _logger.InfoFormat("Adding new teacher to repository: {0}", JsonSerializer.Serialize(teacherPostDTO));

        var teacherDto = await _academicRepository.AddTeacherAsync(teacherPostDTO);

        return teacherDto;
	}

    public async Task<TeacherResponseDTO> GetTeacherById(int id)
    {
        _logger.InfoFormat("Trying to retrieve teacher with ID {0}", id);

        var teacherDto = await _academicRepository.GetTeacherById(id)
            ?? throw new NotFoundException($"Teacher with ID {id} not found.");

        _logger.InfoFormat("Mapping teacher entity to DTO with ID {0}", id);

        return teacherDto;
	}

    public async Task<List<SpecialisationResponseDTO>> GetAllSpecialisations()
    {
        _logger.InfoFormat("Retrieving all specialisations from repository");

        return await _academicRepository.GetAllSpecialisationsAsync();
    }

    public async Task<EnrollmentResponseDTO?> GetEnrollmentById(int enrollmentId)
    {
        _logger.InfoFormat("Trying to retrieve enrollment with ID {0}", enrollmentId);
        return await _academicRepository.GetEnrollmentByIdAsync(enrollmentId)
            ?? throw new NotFoundException($"Enrollment with ID {enrollmentId} not found.");
	}

    public async Task<TeacherResponseDTO?> GetTeacherByUserId(int userId)
    {
        _logger.InfoFormat("Trying to retrieve teacher for user with ID {0}", userId);
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"User with ID {userId} not found.");

        if(user.Role != UserRole.Teacher)
        {
            throw new EntityValidationException($"User with ID {userId} is not a teacher.");
        }

        return await _academicRepository.GetTeacherByUserId(userId)
            ?? throw new NotFoundException($"Teacher for user with ID {userId} not found.");
    }

    public async Task<List<EnrollmentResponseDTO>> GetStudentsByStudentGroup(int studentGroupId)
    {
        _logger.InfoFormat("Trying to retrieve list of students from group with ID {0}", studentGroupId);
        var studentGroupDto = await _academicRepository.GetGroupByIdAsync(studentGroupId)
                              ?? throw new NotFoundException($"StudentGroup with ID {studentGroupId} not found.");
        return await _academicRepository.GetEnrollmentByGroup(studentGroupId);
    }

    public async Task<List<FacultyResponseDTO>> GetAllFaculties()
    {
        _logger.InfoFormat("Retrieving all faculties from repository");
        return await _academicRepository.GetAllFacultiesAsync();
    }

    public Task<List<TeacherResponseDTO>> GetAllTeachersByFacultyId(int facultyId)
    {
        _logger.InfoFormat("Retrieving all teachers for faculty with ID {0}", facultyId);
        return _academicRepository.GetAllTeachersByFacultyId(facultyId);
    }

    public async Task<BulkPromotionResultDTO> CreatePromotionBulk(PromotionPostDTO promotionDto, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new EntityValidationException(["File is empty or not provided."]);
        }

        if (file.Length > 100_000_000)
        {
            throw new EntityValidationException(["File size exceeds the maximum limit of 100 MB."]);
        }

        var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new EntityValidationException(["Unsupported file type. Allowed: .csv, .xlsx"]);
        }

        var parseList = new List<(int Row, BulkEnrollmentItem Dto)>();

        if (fileExtension == ".csv")
        {
            parseList = ParseUserCsvFile(file);
        }
        else
        {
            parseList = ParseUserExcelFile(file);
        }
        
        var promotionValidator = _validatorFactory.Get<PromotionPostDTO>();
        var promotionValidationResult = await promotionValidator.ValidateAsync(promotionDto);

        if (!promotionValidationResult.IsValid)
        {
            throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(promotionValidationResult.Errors));
        }

        (var resultItems, var isValid) = await ValidateEnrollmentsFile(parseList, promotionDto);

        if (!isValid)
        {
            return new BulkPromotionResultDTO { Enrollments = resultItems };
        }

        var finalItems = new List<BulkEnrollmentItemResultDTO>();
        var enrollemntIdsByEmail = new Dictionary<string, int>();

        var generatedPromotionDto = await GenerateBulkPromotionGroups(parseList, promotionDto);
        var createdPromotion = await AddBulkPromotionAsync(generatedPromotionDto, enrollemntIdsByEmail);

        foreach (var (row, dto) in parseList)
        {
            finalItems.Add(new BulkEnrollmentItemResultDTO
            {
                Row = row,
                Email = dto.UserEmail,
                IsValid = true,
                CreatedEnrollmentId = enrollemntIdsByEmail[dto.UserEmail],
            });
        }

        return new BulkPromotionResultDTO { Promotion = createdPromotion, Enrollments = finalItems };
    }

    private async Task<PromotionResponseDTO> AddBulkPromotionAsync(BulkPromotionPostDTO bulkDto, Dictionary<string, int> enrollmentIdsByEmail)
    {
        var promotionPost = new PromotionPostDTO
        {
            StartYear = bulkDto.StartYear,
            EndYear = bulkDto.EndYear,
            SpecialisationId = bulkDto.SpecialisationId
        };

        var createdPromotion = await _academicRepository.AddPromotionAsync(promotionPost);

        foreach (var group in bulkDto.Groups)
        {
            var groupPost = new StudentGroupPostDTO
            {
                Name = group.Name,
                GroupYearId = createdPromotion.Id
            };
            var createdGroup = await _academicRepository.AddGroupAsync(groupPost);

            foreach (var sub in group.SubGroups)
            {
                var subPost = new StudentSubGroupPostDTO
                {
                    Name = sub.Name,
                    StudentGroupId = createdGroup.Id
                };
                var createdSub = await _academicRepository.AddSubGroupAsync(subPost);

                foreach (var enrollment in sub.Enrollments)
                {
                    var user = await _userRepository.GetByEmailAsync(enrollment.UserEmail) 
                        ?? throw new NotFoundException($"User with email {enrollment.UserEmail} not found.");

                    var enrollmentPost = new EnrollmentPostDTO
                    {
                        SubGroupId = createdSub.Id,
                        UserId = user.Id
                    };

                    var createdEnrollment = await _academicRepository.AddEnrollmentAsync(enrollmentPost);

                    enrollmentIdsByEmail[enrollment.UserEmail] = createdEnrollment.Id;
                }
            }
        }

        createdPromotion = await _academicRepository.GetPromotionByIdAsync(createdPromotion.Id)
            ?? throw new NotFoundException($"Promotion with ID {createdPromotion.Id} not found after creation.");

        return createdPromotion;
    }

    private async Task<BulkPromotionPostDTO> GenerateBulkPromotionGroups(List<(int Row, BulkEnrollmentItem Dto)> enrollmentList, PromotionPostDTO promotionDto)
    {
        var enrollemtsCount = enrollmentList.Count;
        enrollmentList.Sort((a, b) => string.Compare(a.Dto.UserEmail, b.Dto.UserEmail, StringComparison.OrdinalIgnoreCase));
        var existingSpecialisation = await _academicRepository.GetSpecialisationByIdAsync(promotionDto.SpecialisationId) 
            ?? throw new NotFoundException($"Specialisation with ID {promotionDto.SpecialisationId} not found.");

        var groupNamePrefix = string.Concat(existingSpecialisation.Name
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => char.ToUpperInvariant(s[0])));

        var numberOfGroups = (int)Math.Ceiling(enrollemtsCount / (decimal)Constants.MaxStudentsInGroup);

        int GetNumberOfEnrollemntsInGroup(int groupIndex)
        {
            if (groupIndex < numberOfGroups - 1)
            {
                return Constants.MaxStudentsInGroup;
            }
            return enrollemtsCount - (Constants.MaxStudentsInGroup * (numberOfGroups - 1));
        }

        var groups = Enumerable.Range(1, numberOfGroups)
            .Select((g, i) => new BulkGroupItem
            {
                Name = $"{groupNamePrefix}{i + 1}",
                SubGroups = Enumerable.Range(1, GetNumberOfEnrollemntsInGroup(i) > Constants.MaxStudentsInSubGroup ? 2 : 1)
                    .Select(sg => new BulkSubGroupItem
                    {
                        Name = $"{groupNamePrefix}{i + 1}/{sg}",
                        Enrollments = []
                    })
                    .ToList()
            })
            .ToList();

        // if you want to refactor this, be my guest
        int groupIndex = 0;
        foreach (var group in groups)
        {
            int subGroupIndex = 0;
            foreach (var subGroup in group.SubGroups)
            {
                for (int i = (groupIndex * 2 + subGroupIndex) * Constants.MaxStudentsInSubGroup; 
                    i < (groupIndex * 2 + subGroupIndex + 1) * Constants.MaxStudentsInSubGroup && i < enrollmentList.Count; i++)
                {
                    subGroup.Enrollments.Add(enrollmentList[i].Dto);
                }
                subGroupIndex++;
            }
            groupIndex++;
        }

        return new BulkPromotionPostDTO
        {
            StartYear = promotionDto.StartYear,
            EndYear = promotionDto.EndYear,
            SpecialisationId = promotionDto.SpecialisationId,
            Groups = groups
        };
    }

    private async Task<(List<BulkEnrollmentItemResultDTO> resultItems, bool isValid)>
        ValidateEnrollmentsFile(List<(int Row, BulkEnrollmentItem Dto)> list, PromotionPostDTO promotionDto)
    {
        var validator = _validatorFactory.Get<BulkEnrollmentItem>();
        var resultItems = new List<BulkEnrollmentItemResultDTO>();

        if (list.Count == 0)
        {
            return (resultItems, false);
        }

        foreach (var (row, dto) in list)
        {
            // check if user with email exists
            var item = new BulkEnrollmentItemResultDTO { Row = row, Email = dto.UserEmail };
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                item.IsValid = false;
                item.Errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
            }

            // check if user already has an enrollemt at the faculty
            var existingFaculty = await _academicRepository.GetSpecialisationFaculty(promotionDto.SpecialisationId);

            if (existingFaculty == null)
            {
                item.IsValid = false;
                item.Errors.Add($"Specialisation with ID {promotionDto.SpecialisationId} does not have an associated faculty.");
                resultItems.Add(item);
                continue;
            }

            var existingEnrollemtList = await _academicRepository.GetUserEnrollemtsFromFaculty(dto.UserEmail, existingFaculty.Id);

            if (existingEnrollemtList.Count != 0)
            {
                item.IsValid = false;
                item.Errors.Add($"User with email {dto.UserEmail} is already enrolled in faculty {existingFaculty.Name}.");
            }

            resultItems.Add(item);
        }

        // check for duplicate emails inside file
        var emailGroups = list
            .Select(p => new { p.Row, Email = p.Dto.UserEmail?.Trim().ToLowerInvariant() })
            .Where(x => !string.IsNullOrEmpty(x.Email))
            .GroupBy(x => x.Email)
            .Where(g => g.Count() > 1);

        foreach (var group in emailGroups)
        {
            foreach (var entry in group)
            {
                var item = resultItems.First(i => i.Row == entry.Row);
                item.IsValid = false;
                item.Errors.Add("Duplicate email inside file.");
            }
        }

        // final validation result
        if (resultItems.Any(i => i.Errors.Count > 0))
        {
            foreach (var it in resultItems)
            {
                it.IsValid = it.Errors.Count == 0;
            }

            return (resultItems, false);
        }

        return (resultItems, true);
    }

    private static List<(int Row, BulkEnrollmentItem Dto)> ParseUserCsvFile(IFormFile file)
    {
        var dtoList = new List<(int Row, BulkEnrollmentItem Dto)>();

        using var reader = new StreamReader(file.OpenReadStream());
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
            HeaderValidated = null
        };

        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<BulkEnrollmentItemMap>();

        try
        {
            if (!csv.Read())
            {
                throw new EntityValidationException(["CSV file is empty."]);
            }

            csv.ReadHeader();
            var headers = csv.HeaderRecord ?? [];
            var headerSet = new HashSet<string>(headers, StringComparer.InvariantCultureIgnoreCase);

            var map = new BulkEnrollmentItemMap();
            var expectedSamples = map.MemberMaps
                .Select(m => m.Data.Names.FirstOrDefault())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();

            var anyMappedHeaderPresent = map.MemberMaps
                .SelectMany(m => m.Data.Names)
                .Any(name => headerSet.Contains(name));


            if (!anyMappedHeaderPresent)
            {
                var sampleList = expectedSamples.Count > 0 ? string.Join(", ", expectedSamples) : "";
                throw new EntityValidationException([$"CSV headers are invalid or missing. Expected headers: {sampleList}."]);
            }

            var records = csv.GetRecords<BulkEnrollmentItem>().ToList();

            for (int i = 0; i < records.Count; i++)
            {
                dtoList.Add((i + 2, records[i]));
            }

            return dtoList;
        }
        catch (CsvHelperException ex)
        {
            throw new EntityValidationException([$"Failed to parse CSV file: {ex.Message}"]);
        }
    }

    private static List<(int Row, BulkEnrollmentItem dto)> ParseUserExcelFile(IFormFile file)
    {
        var dtoList = new List<(int Row, BulkEnrollmentItem Dto)>();

        using var workbook = new XLWorkbook(file.OpenReadStream());
        var workSheet = workbook.Worksheets.First();
        var headerCells = workSheet.Row(1).CellsUsed().ToList();
        var headerMap = headerCells.Select((c, idx) => new { Name = c.GetString().Trim(), Index = idx + 1 })
                                   .ToDictionary(x => x.Name, x => x.Index, StringComparer.InvariantCultureIgnoreCase);

        var map = new BulkEnrollmentItemMap();
        var propertyIndex = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        foreach (var memberMap in map.MemberMaps)
        {
            var propName = memberMap.Data.Member?.Name;
            if (string.IsNullOrWhiteSpace(propName))
            {
                continue;
            }

            foreach (var candidate in memberMap.Data.Names)
            {
                if (headerMap.TryGetValue(candidate, out var index))
                {
                    propertyIndex[propName] = index;
                    break;
                }
            }
        }

        int lastRow = workSheet.LastRowUsed()!.RowNumber();

        for (int r = 2; r <= lastRow; r++)
        {
            string? GetCellByProp(string prop)
            {
                if (!propertyIndex.TryGetValue(prop, out var idx))
                {
                    return null;
                }

                return workSheet.Row(r).Cell(idx).GetString()?.Trim();
            }

            var dto = new BulkEnrollmentItem { UserEmail = GetCellByProp(nameof(BulkEnrollmentItem.UserEmail))! };

            dtoList.Add((r, dto));
        }

        return dtoList;
    }
}
